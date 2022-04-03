using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public class Matrix<T>
    {
        private readonly T _defaultValue;
        public int M { get; private set; }
        public int N { get; private set; }
        private List<List<T>> Data;
        

        public Matrix(int m, int n, T defaultValue)
        {
            _defaultValue = defaultValue;
            M = m;
            N = n;
            Data = new List<List<T>>(M);
            foreach (var i in Enumerable.Range(0, M))
            {
                Data[i] = Enumerable.Repeat(_defaultValue, N).ToList();
            }
        }

        public virtual void InsertRow(int i)
        {
            var newRow = Enumerable.Repeat(_defaultValue, N).ToList();
            if (i < 0)
            {
                Data.Add(newRow);
            }
            else
            {
                Data.Insert(i, newRow);
            }

            ++M;
        }

        public virtual void InsertColumn(int i)
        {
            foreach (var row in Data)
            {
                if (i < 0)
                {
                    row.Add(_defaultValue);
                }
                else
                {
                    row.Insert(i, _defaultValue);
                }
            }

            ++N;
        }
        
        public virtual void RemoveRow(int i)
        {
            Data.RemoveAt(i);
            --M;
        }

        public virtual void RemoveColumn(int i)
        {
            foreach (var row in Data)
            {
                row.RemoveAt(i);
            }

            --N;
        }

        public T Get(int i, int j)
        {
            return Data[i][j];
        }

        public List<T> GetRow(int i)
        {
            return Data[i];
        }
        
        public void Set(int i, int j, T value)
        {
            Data[i][j] = value;
        }
    }

    public class QuadMatrix<T>: Matrix<T>
    {

        public QuadMatrix(int m, T defaultValue): base(m, m, defaultValue)
        {
        }

        public override void InsertColumn(int i)
        {
            Insert(i);
        }

        public override void InsertRow(int i)
        {
            Insert(i);
        }

        public override void RemoveColumn(int i)
        {
            Remove(i);
        }

        public override void RemoveRow(int i)
        {
            Remove(i);
        }

        public void Insert(int i)
        {
            base.InsertColumn(i);
            base.InsertRow(i);
            Debug.Assert(M == N);
        }

        public void Remove(int i)
        {
            base.RemoveRow(i);
            base.RemoveColumn(i);
            Debug.Assert(M == N);
        }
    }
}