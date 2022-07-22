using System.Collections.Generic;

namespace api.config.importer
{
    internal sealed class Row : IRow
    {
        private readonly IList<object> _values;

        public Row(IList<object> values)
        {
            _values = values;
        }

        #region IRow

        int IRow.Length => _values.Count;

        object IRow.this[int index] => _values[index];

        #endregion
    }
}
