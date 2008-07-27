using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace HamlEditor
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideLoadKey(Constants.PLKMinEdition, Constants.PLKProductVersion, Constants.PLKProductName, Constants.PLKCompanyName, Constants.PLKResourceID)]
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0Exp")]
    [ProvideService(typeof(HamlLanguageService), ServiceName = "Haml")]
    [ProvideLanguageService(typeof(HamlLanguageService), "Haml", 100,
        DefaultToInsertSpaces = true,
        EnableCommenting = true)]
    [ProvideLanguageExtension(typeof(HamlLanguageService), ".haml")]
    [Guid(Constants.PackageGuid)]
    public sealed class HamlPackage : Package, IDisposable
    {
        private HamlLanguageService langService;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void Initialize()
        {
            base.Initialize();

            langService = new HamlLanguageService();
            langService.SetSite(this);

            var sc = (IServiceContainer)this;
            sc.AddService(typeof(HamlLanguageService), langService, true);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (langService != null)
                    {
                        langService.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }

}