using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ProjectFootballSim.Common.Features;

public static class EfCoreExtensions
{
    public static bool IsUniqueViolation(this DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlEx &&
               (sqlEx.Number == 2627 || sqlEx.Number == 2601);
    }
}
