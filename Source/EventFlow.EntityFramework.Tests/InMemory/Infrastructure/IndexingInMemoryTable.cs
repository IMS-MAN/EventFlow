// The MIT License (MIT)
// 
// Copyright (c) 2015-2024 Rasmus Mikkelsen
// https://github.com/eventflow/EventFlow
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Microsoft.EntityFrameworkCore.InMemory.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;

namespace EventFlow.EntityFramework.Tests.InMemory.Infrastructure
{
#if NETCOREAPP3_1
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Only for test")]
    public class IndexingInMemoryTable : IInMemoryTable
    {
        private readonly IIndex[] _indexDefinitions;
        private readonly HashSet<IndexEntry>[] _indexes;
        private readonly IInMemoryTable _innerTable;

        public IndexingInMemoryTable(IInMemoryTable innerTable, IIndex[] indexDefinitions)
        {
            _innerTable = innerTable;
            _indexDefinitions = indexDefinitions;
            _indexes = _indexDefinitions.Select(i => new HashSet<IndexEntry>()).ToArray();
        }

        public IReadOnlyList<object[]> SnapshotRows()
        {
            return _innerTable.SnapshotRows();
        }

        public void Create(IUpdateEntry entry)
        {
            var indexEntries = _indexDefinitions
                .Select(d => d.Properties.Select(entry.GetCurrentValue).ToArray())
                .Select(values => new IndexEntry(values))
                .ToArray();

            if (indexEntries.Select((item, i) => _indexes[i].Contains(item)).Any(contains => contains))
                throw new DbUpdateException("Error while updating.", new Exception("Unique constraint violated."));

            _innerTable.Create(entry);

            indexEntries.Select((item, i) => _indexes[i].Add(item)).ToArray();
        }

        public void Delete(IUpdateEntry entry)
        {
            _innerTable.Delete(entry);
        }

        public void Update(IUpdateEntry entry)
        {
            _innerTable.Update(entry);
        }

        public InMemoryIntegerValueGenerator<TProperty> GetIntegerValueGenerator<TProperty>(IProperty property)
        {
            return _innerTable.GetIntegerValueGenerator<TProperty>(property);
        }

        private struct IndexEntry
        {
            private readonly object[] _values;

            public IndexEntry(object[] values)
            {
                _values = values;
            }

            public bool Equals(IndexEntry other)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(_values, other._values);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is IndexEntry entry && Equals(entry);
            }

            public override int GetHashCode()
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(_values);
            }
        }
    }
#elif NET6_0
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Only for test")]
    public class IndexingInMemoryTable : IInMemoryTable
    {
        private readonly IIndex[] _indexDefinitions;
        private readonly HashSet<IndexEntry>[] _indexes;
        private readonly IInMemoryTable _innerTable;

        public IEnumerable<object[]> Rows => _innerTable.Rows;

        public IInMemoryTable BaseTable => _innerTable.BaseTable;

        public IEntityType EntityType => _innerTable.EntityType;

        public IndexingInMemoryTable(IInMemoryTable innerTable, IIndex[] indexDefinitions)
        {
            _innerTable = innerTable;
            _indexDefinitions = indexDefinitions;
            _indexes = _indexDefinitions.Select(i => new HashSet<IndexEntry>()).ToArray();
        }

        public IReadOnlyList<object[]> SnapshotRows()
        {
            return _innerTable.SnapshotRows();
        }

        public void Create(IUpdateEntry entry)
        {
            var indexEntries = _indexDefinitions
                .Select(d => d.Properties.Select(entry.GetCurrentValue).ToArray())
                .Select(values => new IndexEntry(values))
                .ToArray();

            if (indexEntries.Select((item, i) => _indexes[i].Contains(item)).Any(contains => contains))
                throw new DbUpdateException("Error while updating.", new Exception("Unique constraint violated."));

            _innerTable.Create(entry);

            indexEntries.Select((item, i) => _indexes[i].Add(item)).ToArray();
        }

        public void Delete(IUpdateEntry entry)
        {
            _innerTable.Delete(entry);
        }

        public void Update(IUpdateEntry entry)
        {
            _innerTable.Update(entry);
        }

        public InMemoryIntegerValueGenerator<TProperty> GetIntegerValueGenerator<TProperty>(IProperty property, IReadOnlyList<IInMemoryTable> tables)
        {
            return _innerTable.GetIntegerValueGenerator<TProperty>(property, tables);
        }

        public void BumpValueGenerators(object[] row)
        {
            _innerTable.BumpValueGenerators(row);
        }

        private struct IndexEntry
        {
            private readonly object[] _values;

            public IndexEntry(object[] values)
            {
                _values = values;
            }

            public bool Equals(IndexEntry other)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(_values, other._values);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is IndexEntry entry && Equals(entry);
            }

            public override int GetHashCode()
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(_values);
            }
        }

    }
#elif NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Only for test")]
    public class IndexingInMemoryTable : IInMemoryTable
    {
        private readonly IIndex[] _indexDefinitions;
        private readonly HashSet<IndexEntry>[] _indexes;
        private readonly IInMemoryTable _innerTable;

        public IEnumerable<object[]> Rows => _innerTable.Rows;

        public IInMemoryTable BaseTable => _innerTable.BaseTable;

        public IndexingInMemoryTable(IInMemoryTable innerTable, IIndex[] indexDefinitions)
        {
            _innerTable = innerTable;
            _indexDefinitions = indexDefinitions;
            _indexes = _indexDefinitions.Select(i => new HashSet<IndexEntry>()).ToArray();
        }

        public IReadOnlyList<object[]> SnapshotRows()
        {
            return _innerTable.SnapshotRows();
        }

        public void Create(IUpdateEntry entry, IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
        {
            var indexEntries = _indexDefinitions
                .Select(d => d.Properties.Select(entry.GetCurrentValue).ToArray())
                .Select(values => new IndexEntry(values))
                .ToArray();

            if (indexEntries.Select((item, i) => _indexes[i].Contains(item)).Any(contains => contains))
                throw new DbUpdateException("Error while updating.", new Exception("Unique constraint violated."));

            _innerTable.Create(entry, updateLogger);

            indexEntries.Select((item, i) => _indexes[i].Add(item)).ToArray();
        }

        public void Delete(IUpdateEntry entry, IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
        {
            _innerTable.Delete(entry, updateLogger);
        }

        public void Update(IUpdateEntry entry, IDiagnosticsLogger<DbLoggerCategory.Update> updateLogger)
        {
            _innerTable.Update(entry, updateLogger);
        }

        public InMemoryIntegerValueGenerator<TProperty> GetIntegerValueGenerator<TProperty>(IProperty property, IReadOnlyList<IInMemoryTable> tables)
        {
            return _innerTable.GetIntegerValueGenerator<TProperty>(property, tables);
        }

        public void BumpValueGenerators(object[] row)
        {
            _innerTable.BumpValueGenerators(row);
        }

        private struct IndexEntry
        {
            private readonly object[] _values;

            public IndexEntry(object[] values)
            {
                _values = values;
            }

            public bool Equals(IndexEntry other)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(_values, other._values);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is IndexEntry entry && Equals(entry);
            }

            public override int GetHashCode()
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(_values);
            }
        }
    }
#endif
}