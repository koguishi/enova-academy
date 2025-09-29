#!/bin/bash
# init-localstack.sh

echo "Iniciando a configuração dos recursos do LocalStack..."

# Cria a fila SQS 'payment-queue'
awslocal sqs create-queue --queue-name payment-queue

echo "Fila 'payment-queue' criada com sucesso!"