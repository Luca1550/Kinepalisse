using Dapper;
using System.Data;

namespace Kinepalisse.Api.Data;

// MySQL retourne les dates comme DateTime. Ce handler convertit automatiquement
// DateTime → DateOnly pour les propriétés Dapper.
public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
        => parameter.Value = value.ToDateTime(TimeOnly.MinValue);

    public override DateOnly Parse(object value)
        => DateOnly.FromDateTime(Convert.ToDateTime(value));
}
