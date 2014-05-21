using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClarityServicesTestUnits.LVFWS;





namespace ClarityServicesTestUnits {
    class Program {
        //
        static void Main(string[] args) {

            //LedorVadorFlowServiceTestUnit();
            LedorVadorServiceASMXTestUnit();

        }

        public static void LedorVadorServiceASMXTestUnit() {
            Console.ReadKey();
        }

        public static void LedorVadorFlowServiceTestUnit() {
            
            LedorVadorFlowServiceClient service = new LedorVadorFlowServiceClient();

            //LedorVadorFlowService localService = new LedorVadorFlowService();

           // ClarityServices.LedorVador.LedorVadorFlowService service = new ClarityServices.LedorVador.LedorVadorFlowService();
            List<LedorVadorFlowPackage> packages = new List<LedorVadorFlowPackage>();
            //for (int x = 0; x < 10; x++) {
            packages.Add(new LedorVadorFlowPackage() {
                histClinica = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HistClinica")),
                razonSocial = ConfigurationManager.AppSettings.Get("RazonSocial"),
                empresaPropia = Convert.ToInt32(ConfigurationManager.AppSettings.Get("EmpresaID_Propia")),
                documento_externId = ConfigurationManager.AppSettings.Get("Documento_ExternID"),
                establecimiento_numero = ConfigurationManager.AppSettings.Get("Establecimiento_Numero"),
                documento_numero = ConfigurationManager.AppSettings.Get("Documento_Numero"),
                fecha = DateTime.Now,
                observacion = ConfigurationManager.AppSettings.Get("Observacion"),
                tipoServicioId = Convert.ToInt32(ConfigurationManager.AppSettings.Get("TipoServicioID")),
                tipoImpuestoId_Iva = Convert.ToInt32(ConfigurationManager.AppSettings.Get("TipoImpuestoID_IVA")),
                cantidad = Convert.ToDouble(ConfigurationManager.AppSettings.Get("Cantidad")),
                importeUnitario = Convert.ToDouble(ConfigurationManager.AppSettings.Get("ImporteUnitario"))
            });
            //}
            LedorVadorRequest request = new LedorVadorRequest() {
                login = ConfigurationManager.AppSettings.Get("login"),
                password = ConfigurationManager.AppSettings.Get("pass"),
                packages = packages
            };

            try { 
            
                service.Endpoint.ListenUri = new Uri(ConfigurationManager.AppSettings.Get("EnpointAdress"));

                

            LedorVadorResponse response = service.SendPackages(request);
//                 LedorVadorResponse response = localService.SendPackages(request);
            Console.WriteLine(response.responseMessage);
            } catch (Exception e) {
                Console.WriteLine("[LedorVadorFlowService TestUnit Exception][Posible EnpointAdress nulo][" + e.ToString().Substring(0, 300) + "]");
            }
            Console.ReadKey();
        }
        //
    }
}


