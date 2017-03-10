using System;

namespace LaTexInclude.Model
{
    public interface IDataService
    {
        void GetData(Action<DataItem, Exception> callback);
    }
}