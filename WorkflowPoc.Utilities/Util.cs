using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowPoc.Utilities
{
    public class Util
    {
        /// <summary>
        /// Verifica se o fornecedor está ativo
        /// </summary>
        /// <param name="idSupplier"></param>
        public bool IsSupplierActive(int idSupplier)
        {
            return true;
        }

        /// <summary>
        /// Verifica se o cliente está ativo
        /// </summary>
        /// <param name="idSupplier"></param>
        public bool IsCustomerActive(int idCustomer)
        {
            return true;
        }

        /// <summary>
        /// Verifica se o cliente está ativo
        /// </summary>
        /// <param name="idSupplier"></param>
        public CustomerInfo GetContactCustomer(int idCustomer)
        {
            return new CustomerInfo() 
            { 
                Id = idCustomer,
                Name = "Luã Govinda Mendes Souza",
                Email = "govinda777@gmail.com",
                Cel = "11 9-6394-8892"
            };
        }

        /// <summary>
        /// Verifica se o cliente está ativo
        /// </summary>
        /// <param name="idSupplier"></param>
        public void SendEmail(StringBuilder content, string to)
        {

        }

    }

    public class CustomerInfo
    {
        public CustomerInfo()
        {

        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cel { get; set; }
    }
}
