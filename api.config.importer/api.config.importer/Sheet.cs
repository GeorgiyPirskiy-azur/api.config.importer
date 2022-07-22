using System.Collections.Generic;

namespace api.config.importer
{
    internal sealed class Sheet : ISheet
    {
        private readonly Dictionary<int, IRow> _rows = new Dictionary<int, IRow>();
        private readonly string _name;
        private readonly IList<IList<object>> _values;

        public Sheet(string name, IList<IList<object>> values)
        {
            _name = name;
            _values = values;
        }

        private IRow GetRow(int index)
        {
            var rowNotFound = !_rows.TryGetValue(index, out var row);
            if (rowNotFound)
            {
                row = new Row(_values[index]);
                _rows.Add(index, row);
            }
            return row;
        }

        #region ISheet

        string ISheet.Name => _name;

        int ISheet.Length => _values.Count;

        IRow ISheet.this[int index] => GetRow(index);

        #endregion
    }
}
