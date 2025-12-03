using ET.Application.Contracts;
using ET.Application.DTOs;
using ET.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ET.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ExpenseTrackerContext _expenseTrackerContext;
    private readonly ICurrentUserService _currentUserService;

    public DashboardService(ExpenseTrackerContext expenseTrackerContext, ICurrentUserService currentUserService)
    {
        _expenseTrackerContext = expenseTrackerContext;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardModel> GetDashboardData(int userId, int month, string type)
    {
        var dashboard = new DashboardModel();
        dashboard.CategoryExpenses = new List<CategoryExpenseItem>();

        using (var connection = _expenseTrackerContext.Database.GetDbConnection())
        {
            //check connection state before opening it
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "GetDashboardData";
                command.CommandType = CommandType.StoredProcedure;

                var userParam = command.CreateParameter();
                userParam.ParameterName = "@UserId";
                userParam.Value = _currentUserService.GetCurrentUserId();
                command.Parameters.Add(userParam);

                var monthParam = command.CreateParameter();
                monthParam.ParameterName = "@Month";
                monthParam.Value = month;
                command.Parameters.Add(monthParam);

                var typeParam = command.CreateParameter();
                typeParam.ParameterName = "@Type";
                typeParam.Value = type;
                command.Parameters.Add(typeParam);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        dashboard.Summary = new DashboardItem
                        {
                            TotalExpenses = reader.GetDecimal(reader.GetOrdinal("TotalExpenses")),
                            NumberofExpenses = reader.GetInt32(reader.GetOrdinal("NumberofExpenses")),
                            AverageExpense = reader.GetDecimal(reader.GetOrdinal("AverageExpense")),
                            ExpenseCategoriesUsed = reader.GetInt32(reader.GetOrdinal("ExpenseCategoriesUsed")),
                        };
                    }

                    await reader.NextResultAsync();

                    while (await reader.ReadAsync())
                    {
                        dashboard.CategoryExpenses.Add(new CategoryExpenseItem
                        {
                            CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                            CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                            TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"))
                        });
                    }

                    await reader.NextResultAsync();

                    dashboard.MonthlyTrend = new List<MonthlyTrendItem>();

                    while (await reader.ReadAsync())
                    {
                        dashboard.MonthlyTrend.Add(new MonthlyTrendItem
                        {
                            Month = reader.GetString(reader.GetOrdinal("Month")),
                            TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"))
                        });
                    }

                }
            }
        }

        return dashboard;
    }

}
