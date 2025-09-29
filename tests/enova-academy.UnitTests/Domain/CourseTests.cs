using Xunit;
using System;
using enova_academy.Domain.Entities;
using enova_academy.Domain;

namespace enova_academy.UnitTests.Domain;
public class CourseTests
{
    [Fact]
    public void Constructor_ShouldCreateValidCourse_WhenParametersAreValid()
    {
        var course = new Course("Curso .NET", "curso-dotnet", 200m, 50);

        Assert.Equal("Curso .NET", course.Title);
        Assert.Equal("curso-dotnet", course.Slug);
        Assert.Equal(200m, course.Price);
        Assert.Equal(50, course.Capacity);
        Assert.NotEqual(Guid.Empty, course.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_ShouldThrow_WhenTitleIsNullOrEmpty(string? title)
    {
        Assert.Throws<Exception>(() => new Course(title!, "slug", 100m, 10));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_ShouldThrow_WhenSlugIsNullOrEmpty(string? slug)
    {
        Assert.Throws<Exception>(() => new Course("Curso", slug!, 100m, 10));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_ShouldThrow_WhenPriceIsZeroOrNegative(decimal price)
    {
        Assert.Throws<Exception>(() => new Course("Curso", "slug", price, 10));
    }

    [Fact]
    public void Constructor_ShouldAllow_NullCapacity()
    {
        var course = new Course("Curso .NET", "curso-dotnet", 200m, null);
        Assert.Null(course.Capacity);
    }

    [Fact]
    public void Atualizar_ShouldUpdateOnlyProvidedFields()
    {
        var course = new Course("Curso A", "slug-a", 100m, 10);

        course.Atualizar("Curso B", null, null, null);

        Assert.Equal("Curso B", course.Title);
        Assert.Equal("slug-a", course.Slug);
        Assert.Equal(100m, course.Price);
        Assert.Equal(10, course.Capacity);
    }

    [Fact]
    public void Atualizar_ShouldUpdateAllFields()
    {
        var course = new Course("Curso A", "slug-a", 100m, 10);

        course.Atualizar("Curso B", "slug-b", 200m, 20);

        Assert.Equal("Curso B", course.Title);
        Assert.Equal("slug-b", course.Slug);
        Assert.Equal(200m, course.Price);
        Assert.Equal(20, course.Capacity);
    }

    [Fact]
    public void IsFull_ShouldReturnFalse_WhenCapacityIsNull()
    {
        var course = new Course("Curso", "slug", 100m, null);
        Assert.False(course.IsFull());
    }

    [Fact]
    public void IsFull_ShouldReturnTrue_WhenEnrollmentsReachCapacity()
    {
        var course = new Course("Curso", "slug", 100m, 1);
        course.Enrollments!.Add(new Enrollment(Guid.NewGuid(), Guid.NewGuid()));
        Assert.True(course.IsFull());
    }

    [Fact]
    public void StudentFound_ShouldReturnTrue_WhenStudentIsEnrolled()
    {
        var studentId = Guid.NewGuid();
        var course = new Course("Curso", "slug", 100m, 10);
        course.Enrollments!.Add(new Enrollment(studentId, course.Id));

        Assert.True(course.StudentFound(studentId));
    }

    [Fact]
    public void StudentFound_ShouldReturnFalse_WhenStudentIsNotEnrolled()
    {
        var course = new Course("Curso", "slug", 100m, 10);
        Assert.False(course.StudentFound(Guid.NewGuid()));
    }

    [Fact]
    public void Enroll_ShouldAddStudent_WhenCourseNotFullAndStudentNotEnrolled()
    {
        var studentId = Guid.NewGuid();
        var course = new Course("Curso", "slug", 100m, 10);

        var enrollment = new Enrollment(studentId, course.Id);
        course.Enroll(enrollment);

        Assert.Single(course.Enrollments!);
        Assert.True(course.StudentFound(studentId));
    }

    [Fact]
    public void Enroll_ShouldThrow_WhenCourseIsFull()
    {
        var course = new Course("Curso", "slug", 100m, 1);
        course.Enrollments!.Add(new Enrollment(Guid.NewGuid(), Guid.NewGuid()));

        var newEnrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());

        Assert.Throws<CourseCapacityExceededException>(() => course.Enroll(newEnrollment));
    }

    [Fact]
    public void Enroll_ShouldThrow_WhenStudentAlreadyEnrolled()
    {
        var studentId = Guid.NewGuid();
        var course = new Course("Curso", "slug", 100m, 10);
        course.Enrollments!.Add(new Enrollment(studentId, course.Id));

        var newEnrollment = new Enrollment(studentId, course.Id);

        Assert.Throws<EnrollmentAlreadyTakenException>(() => course.Enroll(newEnrollment));
    }
}
