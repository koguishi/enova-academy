using enova_academy.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace enova_academy.Application.Workers;

public class MetricsRefreshWorker(
    IServiceProvider provider
) : BackgroundService
{
    IServiceProvider _provider = provider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await AtualizarMetricasAsync();
        }
    }

    public async Task AtualizarMetricasAsync()
    {
        var totalPorCurso = Metrics.CreateGauge(
            "dominio_matriculas_total",
            "Quantidade total de matrículas por curso",
            new GaugeConfiguration { LabelNames = ["curso"] }
        );

        var percentualPagoPorCurso = Metrics.CreateGauge(
            "dominio_matriculas_percentual_pago",
            "% de matrículas pagas por curso",
            new GaugeConfiguration { LabelNames = ["curso"] }
        );

        using var scope = _provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var agrupado = await (
            from e in db.Enrollments
            join c in db.Courses on e.CourseId equals c.Id
            group e by new { e.CourseId, c.Title } into g
            select new
            {
                CourseId = g.Key.CourseId,
                CourseName = g.Key.Title,
                Total = g.Count(),
                Pagos = g.Count(x => x.Status == "paid"),
                PercentualPagos = g.Count(x => x.Status == "paid") * 100.0 / g.Count()
            }
        ).ToListAsync();

        foreach (var grupo in agrupado)
        {
            totalPorCurso.WithLabels(grupo.CourseName).Set(grupo.Total);
            percentualPagoPorCurso.WithLabels(grupo.CourseName).Set(grupo.PercentualPagos);
        }
    }

}
