using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Interop.Common;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Common.Internal
{
    internal class CategoryEnumerator : IDisposable
    {
        private ICatInformation _catInformation;
        private object _componentCategoriesMgr;
        private bool _disposed; // Flag: Has Dispose already been called?

        public CategoryEnumerator(string host)
        {
            MaxItemsPerRequest = 16;
            _componentCategoriesMgr = Com.CreateInstance(typeof(StdComponentCategoriesMgr).GUID, host, null);
            _catInformation = (ICatInformation) _componentCategoriesMgr;
        }

        public int MaxItemsPerRequest { get; set; }
        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //

            if (_componentCategoriesMgr != null)
            {
                Marshal.ReleaseComObject(_componentCategoriesMgr);
                _componentCategoriesMgr = null;
                _catInformation = null;
            }

            _disposed = true;
        }

        ~CategoryEnumerator()
        {
            Dispose(false);
        }

        public OpcServerCategory[] GetCategories(Guid clsid)
        {
            IEnumGUID catEnumerator = null;

            try
            {
                catEnumerator = _catInformation.EnumImplCategoriesOfClass(clsid);

                var catids = new Guid[MaxItemsPerRequest];
                int fetched;
                var result = new List<OpcServerCategory>();
                do
                {
                    catEnumerator.Next(catids.Length, catids, out fetched);
                    for (var i = 0; i < fetched; i++)
                    {
                        var catid = catids[i];
                        var opcServerCategory = OpcServerCategory.Get(catid);
                        if (opcServerCategory != null)
                            result.Add(opcServerCategory);
                    }
                } while (fetched != 0);

                return result.ToArray();
            }
            finally
            {
                if (catEnumerator != null)
                    Marshal.ReleaseComObject(catEnumerator);
            }
        }
    }
}