using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace RestFulDemo
{
    [ServiceContract]
    public interface IRestService
    {
        //POST operation
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "POST")]
        Person CreatePerson(Person createPerson);

        //Get Operation
        [OperationContract]
        [WebGet(UriTemplate = "")]
        List<Person> GetAllPerson();
        [OperationContract]
        [WebGet(UriTemplate = "{id}")]
        Person GetAPerson(string id);

        //PUT Operation
        [OperationContract]
        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        Person UpdatePerson(string id, Person updatePerson);

        //DELETE Operation
        [OperationContract]
        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        void DeletePerson(string id);
    }
    [DataContract]
    public class Person
    {
        [DataMember]
        public string ID;
        [DataMember]
        public string Name;
        [DataMember]
        public string Age;
    }
}
