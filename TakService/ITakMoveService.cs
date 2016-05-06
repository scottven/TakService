using System.ServiceModel;
using System.ServiceModel.Web;

namespace TakService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITakMoveService" in both code and config file together.
    [ServiceContract(Namespace = "")]
   
    public interface ITakMoveService
    {

        [OperationContract]
        [WebGet]
        string GetMove(string ptn = null, string code = null, int aiLevel = 3, int flatScore = 9000, bool tps = false);

        [OperationContract]
        [WebGet]
        string[][] GetAllMoves(string code, int aiLevel = 3, int flatScore = 9000, bool tps = false);

  //      [OperationContract]
   //     CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
   /* [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }*/
}
