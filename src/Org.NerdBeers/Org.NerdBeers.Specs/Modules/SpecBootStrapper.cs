using System;
using System.IO;
using System.Reflection;
using Nancy.Authentication.Forms;
using Org.NerdBeers.Web.Services;
using Simple.Data.Mocking;

namespace Org.NerdBeers.Specs.Modules
{
    class SpecBootStrapper : NerdBeers.Web.Bootstrapper
    {
        IDBFactory fact = new CustomDBFactory();

        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<IDBFactory>(fact);
            container.Register<IUserMapper>(new UserMapper(fact));
        }

        public dynamic DB
        {
            get {
                return fact.DB();
            }
        }
    }

    class CustomDBFactory : IDBFactory
    {
        bool IsInitialized = false;

        public dynamic DB()
        {
            if (!IsInitialized)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Org.NerdBeers.Specs.Modules._TestDatabase.xml"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd().Replace("/2010 ", "/" + DateTime.Now.Year.ToString() + " ");
                    MockHelper.UseMockAdapter(new XmlMockAdapter(result));
                }
                IsInitialized = true;
            }
            return Simple.Data.Database.Default;
        }
    }
}
