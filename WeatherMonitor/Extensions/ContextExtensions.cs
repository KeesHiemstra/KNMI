using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Extensions
{
  public static class ContextExtensions
  {

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
