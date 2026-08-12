using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ProjectFootballSim.Common.Features.EntityFrameworkCore;

public static class DbUpdateExceptionExtensions
{
    public static bool IsUniqueViolation(this DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlEx &&
               (sqlEx.Number == 2627 || sqlEx.Number == 2601);
    }
}
