using BatchProcessing;
using BatchProcessing.Infraestructure.Database;
using BatchProcessing.Interfaces;
using BatchProcessing.Models;
using BatchProcessing.Repositories;
using BatchProcessing.Services;
using BatchProcessing.Validations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddDbContext<BatchDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IFileReader<TransactionRaw>, FileReaderService>();
builder.Services.AddScoped<ITransactionValidator, TransactionsValidateService>();
builder.Services.AddScoped<IValidator<TransactionProcessed>, TransactionProcessedValidator>();
builder.Services.AddScoped<ITransactionINService<TransactionRaw>, TransactionINService>();
builder.Services.AddScoped<ITransactionProcessedService<TransactionProcessed>, TransactionProcessesService>();
builder.Services.AddScoped<ITransactionINRepository<TransactionRaw>, TransactionINRepository>();
builder.Services.AddScoped<ITransactionProcessesRepository<TransactionProcessed>, TransactionProcessesRepository>();
builder.Services.AddScoped<ILoggerFileService, LoggerFileService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
