using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using Xunit;

namespace VirtoCommerce.News.Tests;

[Trait("Category", "CI")]
public class MigrationTests
{
    [Fact]
    public void CheckMigrationConsistency()
    {
        var mySqlMigrations = GetMigrationTypes(typeof(VirtoCommerce.News.Data.MySql.MySqlDataAssemblyMarker).Assembly);
        var postgreMigrations = GetMigrationTypes(typeof(VirtoCommerce.News.Data.PostgreSql.PostgreSqlDataAssemblyMarker).Assembly);
        var sqlServerMigrations = GetMigrationTypes(typeof(VirtoCommerce.News.Data.SqlServer.SqlServerDataAssemblyMarker).Assembly);

        Assert.True(mySqlMigrations.Count == postgreMigrations.Count, $"MySql migrations count {mySqlMigrations.Count} doesn't match PostgreSQL migrations count {postgreMigrations.Count}");
        Assert.True(sqlServerMigrations.Count == postgreMigrations.Count, $"SqlServer migrations count {sqlServerMigrations.Count} doesn't match PostgreSQL migrations count {postgreMigrations.Count}");
        Assert.True(sqlServerMigrations.Count == mySqlMigrations.Count, $"SqlServer migrations count {sqlServerMigrations.Count} doesn't match MySql migrations count {mySqlMigrations.Count}");
    }

    private IList<Type> GetMigrationTypes(Assembly assembly, DateTime? after = null)
    {
        var result = assembly.GetTypes()
            .Where(x => typeof(Migration).IsAssignableFrom(x));

        if (after != null)
        {
            result = result.Where(x => GetMigrationDate(x) >= after).ToList();
        }

        return result.ToList();
    }

    private DateTime? GetMigrationDate(Type migrationType)
    {
        var migrationAttribute = migrationType.GetCustomAttribute<MigrationAttribute>();

        if (migrationAttribute == null || migrationAttribute.Id == null)
        {
            return null;
        }

        if (DateTime.TryParseExact(migrationAttribute.Id.Split('_')[0], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out var migrationDate))
        {
            return migrationDate;
        }

        return null;
    }
}
