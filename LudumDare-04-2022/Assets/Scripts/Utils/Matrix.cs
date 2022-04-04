using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace Utils
{
    public class Matrix
    {
        private readonly float _defaultValue;
        public int M { get; private set; }
        public int N { get; private set; }
        private List<List<float>> Data;


        public Matrix(int m, int n, float defaultValue)
        {
            _defaultValue = defaultValue;
            M = m;
            N = n;
            Data = Enumerable.Range(0, M).Select(_ => Enumerable.Repeat(_defaultValue, N).ToList()).ToList();
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

        public float Get(int i, int j)
        {
            return Data[i][j];
        }

        public List<float> GetRow(int i)
        {
            return Data[i];
        }

        public void Set(int i, int j, float value)
        {
            Data[i][j] = value;
        }

        public void CloneAndMultiplyElementWise(Matrix other)
        {
            Debug.Assert(M == other.M && N == other.N, "Matrices must be same size.");
            var result = new Matrix(M, N, _defaultValue);
            for (int i = 0; i < M; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    result.Set(i, j, Get(i, j) * other.Get(i, j));
                }
            }
        }

        public void CloneAndMultiplyColumnWise(float[] other)
        {
            Debug.Assert(N == other.Length, "Other must be the length of the columns");
            var result = new Matrix(M, N, _defaultValue);
            for (int i = 0; i < M; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    result.Set(i, j, Get(i, j) * other[j]);
                }
            }
        }
    }

    public class QuadMatrix : Matrix
    {
        public QuadMatrix(int m, float defaultValue) : base(m, m, defaultValue)
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