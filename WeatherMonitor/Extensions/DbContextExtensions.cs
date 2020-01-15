using System;
using System.Data.Entity;
using System.Reflection;

namespace CHi.Extensions
{
  public static class DbContextExtensions
  {

    /// <summary>
    /// Show the private method IsDisposed of 'System.Data.Entity.Internal.InternalContext'.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public static bool IsDisposed(this DbContext dbContext)
    {

      bool result = true;

      Type typeDbContext = typeof(DbContext);
      Type typeInternalContext =
        typeDbContext.Assembly.GetType("System.Data.Entity.Internal.InternalContext");

      FieldInfo fieldInfoInternalContext = 
        typeDbContext.GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance);
      PropertyInfo propertyInfoIsDisposed = typeInternalContext.GetProperty("IsDisposed");
      object internalContext = fieldInfoInternalContext.GetValue(dbContext);

      if (internalContext != null)
      {
        result = (bool)propertyInfoIsDisposed.GetValue(internalContext);
      }

      return result;

    }

  }
}
