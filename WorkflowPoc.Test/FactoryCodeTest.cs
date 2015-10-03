using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.CodeDom;
using WorkflowPoc.Core;

namespace WorkflowPoc.Test
{
    [TestClass]
    public class FactoryCodeTest
    {
        const string NAME_SPACE = "SteveJobs";
        const string CLASS_NAME = "Apple";
        const string METHOD_NAME = "Create";
        const string MEMBER_NAME = "product";
        const string PROPERTY_NAME = "Verssion";

        public FactoryCode factoryCode;
        /*public CodeNamespace customNamespace;
        public CodeTypeDeclaration customClass;
        public CodeCompileUnit customAssembly;
        */
        public FactoryCodeTest()
        {
            factoryCode = new FactoryCode();
        }
        

        public void CreateNamespace()
        {
            CodeNamespace customNamespace;

            customNamespace = factoryCode.CreateNamespace(NAME_SPACE);
        }

        [TestMethod]
        public void CreateImportsTest()
        {
            CodeNamespace customNamespace;

            customNamespace = factoryCode.CreateNamespace(NAME_SPACE);

            factoryCode.CreateImports(ref customNamespace,"System", "System.Text", "System.Linq");
        }

        [TestMethod]
        public void CreateClassTest()
        {
            CodeNamespace customNamespace;
            CodeTypeDeclaration customClass;

            customNamespace = factoryCode.CreateNamespace(NAME_SPACE);

            factoryCode.CreateImports(ref customNamespace, "System", "System.Text", "System.Linq");

            customClass = factoryCode.CreateClass(ref customNamespace, CLASS_NAME);
        }

        [TestMethod]
        public void CreateMemberTest()
        {
            CodeNamespace customNamespace;
            CodeTypeDeclaration customClass;

            customNamespace = factoryCode.CreateNamespace(NAME_SPACE);

            factoryCode.CreateImports(ref customNamespace, "System", "System.Text", "System.Linq");

            customClass = factoryCode.CreateClass(ref customNamespace, CLASS_NAME);

            factoryCode.CreateMember(ref customClass, MEMBER_NAME, typeof(int));
        }

        [TestMethod]
        public void CreatePropertyTest()
        {
            CodeNamespace customNamespace;
            CodeTypeDeclaration customClass;

            customNamespace = factoryCode.CreateNamespace(NAME_SPACE);

            factoryCode.CreateImports(ref customNamespace, "System", "System.Text", "System.Linq");

            customClass = factoryCode.CreateClass(ref customNamespace, CLASS_NAME);

            factoryCode.CreateProperty(ref customClass,PROPERTY_NAME,typeof(int));
        }

        [TestMethod]
        public void CreateMethodTest()
        {
            CodeNamespace customNamespace;
            CodeTypeDeclaration customClass;

            customNamespace = factoryCode.CreateNamespace(NAME_SPACE);

            factoryCode.CreateImports(ref customNamespace, "System", "System.Text", "System.Linq");

            customClass = factoryCode.CreateClass(ref customNamespace, CLASS_NAME);

            string method = @"
            public int "+ METHOD_NAME + @"()
            {
                return 1+1;                
            }";

            factoryCode.CreateMethod(ref customClass,METHOD_NAME,method,null, typeof(int));
        }

        [TestMethod]
        public void SaveAssemblyTest()
        {
            CodeNamespace customNamespace;
            CodeTypeDeclaration customClass;

            customNamespace = factoryCode.CreateNamespace(NAME_SPACE);

            factoryCode.CreateImports(ref customNamespace, "System", "System.Text", "WorkflowPoc.Utilities");

            customClass = factoryCode.CreateClass(ref customNamespace, CLASS_NAME);

            string method = @"return 1+1;";

            factoryCode.CreateMethod(ref customClass, METHOD_NAME, method, null, typeof(int));

            //string method2 = @"return 1+1;";

            //var param = new Dictionary<string,Type>();
            //param.Add("testeParametro", typeof(int));

            //factoryCode.CreateMethod(ref customClass, METHOD_NAME + "2", method2, param, typeof(int));

            //factoryCode.CreateProperty(ref customClass, PROPERTY_NAME, typeof(int));

            factoryCode.CreateMember(ref customClass, MEMBER_NAME, typeof(int));

            factoryCode.CreateEntryPoint(ref customClass);

            string[] reference = new string[] { @"C:\Users\lua.souza\Documents\visual studio 2013\Projects\WorkflowPod\WorkflowPoc.Core\lib\WorkflowPoc.Utilities.dll" };

            factoryCode.SaveAssembly(ref customNamespace, NAME_SPACE, CLASS_NAME, @"C:\temp", reference);
        }

    }
}
