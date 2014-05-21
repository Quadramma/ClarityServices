using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClarityServices.LedorVador {

    public class LedorVadorResponse {
        public List<int> documentosAceptados = new List<int>();
        public List<int> documentosOmitidos = new List<int>();
        public string responseMessage = "";
    }
    public class LedorVadorRequest {
        public string login = "";
        public string password ="";
        public List<LedorVadorFlowPackage> packages = new List<LedorVadorFlowPackage>();
    }
    public class LedorVadorFlowPackage { 
        public int histClinica;
        public string razonSocial;
        public int empresaPropia;
        public string documento_externId;
        public string establecimiento_numero;
        public string documento_numero;
        public DateTime fecha;
        public string observacion;
        public int tipoServicioId;
        public int tipoImpuestoId_Iva;
        public double cantidad;
        public double importeUnitario;
    }
       
    [ServiceContract]
    public interface ILedorVadorFlowService {

        [OperationContract]
        LedorVadorResponse SendPackages(LedorVadorRequest request);


    }

}
