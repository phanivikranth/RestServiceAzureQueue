using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace RestFulDemo
{
    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RestService : IRestService
    {
        List<Person> Persons = new List<Person>();
        int PersonCount = 0;

        public Person CreatePerson(Person createPerson)
        {
            createPerson.ID = (++PersonCount).ToString();
            Persons.Add(createPerson);
            return createPerson;
        }

        public List<Person> GetAllPerson()
        {
            return Persons.ToList();
        }

        public Person GetAPerson(string id)
        {
            return Persons.FirstOrDefault(e => e.ID.Equals(id));
        }

        public Person UpdatePerson(string id, Person updatePerson)
        {
            Person p = Persons.FirstOrDefault(e => e.ID.Equals(id));
            p.Name = updatePerson.Name;
            p.Age = updatePerson.Age;
            return p;
        }

        public void DeletePerson(string id)
        {
            Persons.RemoveAll(e => e.ID.Equals(id));
        }
    }
}
