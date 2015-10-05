using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.CodeDom;
using System.IO;


namespace WorkflowPoc.Core
{
    /// <summary>
    /// Classe Core responsável pele mecanismo de geração de código
    /// </summary>
    public class FactoryCode
    {

        /// <summary>
        /// Cria um novo namespaca
        /// </summary>
        /// <param name="nameSpace">nome do namespace</param>
        /// <returns></returns>
        public CodeNamespace CreateNamespace(string nameSpace)
        {
            //vmsldvmsdlvmsdçlvmsçldmvsçldm
            return new CodeNamespace(nameSpace);
        }

        /// <summary>
        /// Adiciona using no namespace
        /// </summary>
        /// <param name="customNamespace"></param>
        /// <param name="nameSpaces"></param>
        public void CreateImports(ref CodeNamespace customNamespace, params string[] nameSpaces)
        {
            foreach (var item in nameSpaces)
            {
                customNamespace.Imports.Add(new CodeNamespaceImport(item));
            }
        }

        public CodeTypeDeclaration CreateClass(ref CodeNamespace customNamespace, string className)
        {
            CodeTypeDeclaration customClass = new CodeTypeDeclaration();

            //Assign a name for the class
            customClass.Name = className;
            customClass.IsClass = true;
            //Set the Access modifier.
            customClass.Attributes = MemberAttributes.Public;
            //Add the newly created class to the namespace
            customNamespace.Types.Add(customClass);

            return customClass;
        }

        public void CreateMember(ref CodeTypeDeclaration customClass, string memberName, Type type)
        {
            //Provide the type and variable name 
            CodeMemberField memberfield = new CodeMemberField(type, memberName);

            memberfield.Attributes = MemberAttributes.Public;

            //Add the member to the class
            customClass.Members.Add(memberfield);
        }

        public void CreateProperty(ref CodeTypeDeclaration customClass, string memberPropertyName, Type type)
        {
            CodeMemberProperty memberproperty = new CodeMemberProperty();

            //Name of the property
            memberproperty.Name = memberPropertyName;

            //Data type of the property
            memberproperty.Type = new CodeTypeReference(type);

            //Access modifier of the property
            memberproperty.Attributes = MemberAttributes.Public;

            //Add the property to the class
            customClass.Members.Add(memberproperty);

            //Add the code-snippets to the property.  
            //If required, we can also add some custom validation code.
            //using the CodeSnippetExpression class.

            //Provide the return <propertyvalue> statement – For getter
            CodeSnippetExpression getsnippet = new CodeSnippetExpression(string.Concat("return ", memberPropertyName));

            //Assign the new value to the property – For setter
            CodeSnippetExpression setsnippet = new CodeSnippetExpression(string.Concat(memberPropertyName, "=", "value"));

            //Add the code snippets into the property
            memberproperty.GetStatements.Add(getsnippet);
            memberproperty.SetStatements.Add(setsnippet);
        }

        public void CreateMethod(ref CodeTypeDeclaration customClass, string methodName, string methodCode, Dictionary<string, Type> parameters, Type returnType)
        {
            //Create an object of the CodeMemberMethod
            CodeMemberMethod method = new CodeMemberMethod();

            //Assign a name for the method.
            method.Name = methodName;

            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    //create parameters
                    CodeParameterDeclarationExpression cpd = new CodeParameterDeclarationExpression(item.Value, item.Key);

                    //Add the parameters to the method.
                    method.Parameters.AddRange(new CodeParameterDeclarationExpression[] { cpd });

                    CreateMember(ref customClass, GetParameterName(methodName, item.Key), typeof(int));
                }
            }
            //Provide the return type for the method.
            CodeTypeReference ctr = new CodeTypeReference(returnType);

            //Assign the return type to the method.
            method.ReturnType = ctr;

            CodeSnippetExpression snippet = new CodeSnippetExpression(methodCode);

            //Convert the snippets into Expression statements.

            CodeExpressionStatement stmt = new CodeExpressionStatement(snippet);

            //Add the expression statements to the method.
            method.Statements.Add(stmt);

            //Provide the access modifier for the method.
            method.Attributes = MemberAttributes.Public;

            //Finally add the method to the class.
            customClass.Members.Add(method);
        }

        public string GetParameterName(string methodName, string parameterName)
        {
            return string.Concat(methodName, parameterName);
        }

        public void CreateEntryPoint(ref CodeTypeDeclaration customClass)
        {
            //Create an object and assign the name as “Main”
            CodeEntryPointMethod main = new CodeEntryPointMethod();
            main.Name = "Main";

            //Mark the access modifier for the main method as Public and //static
            main.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            //Provide defenition to the main method.  
            //Create an object of the “Cmyclass” and invoke the method
            //by passing the required parameters.
            CodeSnippetExpression exp = new CodeSnippetExpression(CallingMethod(customClass));

            //Create expression statements for the snippets
            CodeExpressionStatement ces = new CodeExpressionStatement(exp);

            //Add the expression statements to the main method.		
            main.Statements.Add(ces);

            //Add the main method to the class
            customClass.Members.Add(main);
        }

        public string CallingMethod(CodeTypeDeclaration customClass)
        {
            List<string> callingMethod = new List<string>();

            callingMethod.Add(ReadyNewInstanceClass(customClass));

            customClass.Members.Cast<CodeTypeMember>()
                               .ToList()
                               .Where(x => x.GetType() == typeof(CodeMemberMethod))
                               .ToList()
                               .ForEach(x => {
                                   callingMethod.Add(ReadyCall(customClass, (CodeMemberMethod)x));
                               });

            return string.Join(" ", callingMethod.ToArray());
        }

        public string ReadyNewInstanceClass(CodeTypeDeclaration customClass)
        {
            return string.Concat(customClass.Name, " ", customClass.Name.ToLower(), "= new ", customClass.Name, "();");
        }

        public string ReadyCall(CodeTypeDeclaration customClass, CodeMemberMethod codeMemberMethod)
        {
            string call = string.Empty;

            call += GetReturnDeclaration(codeMemberMethod);

            call += string.Concat(customClass.Name.ToLower(), ".", codeMemberMethod.Name, GetParameterDeclaration(customClass, codeMemberMethod));

            return call;
        }

        public string GetReturnDeclaration(CodeMemberMethod codeMemberMethod)
        {
            string declaration = string.Concat(codeMemberMethod.ReturnType.BaseType, " result", codeMemberMethod.Name, "=");

            return declaration;
        }

        public string GetParameterDeclaration(CodeTypeDeclaration customClass, CodeMemberMethod codeMemberMethod)
        {
            string parameters = "(";

            codeMemberMethod.Parameters.Cast<CodeParameterDeclarationExpression>()
                                       .ToList()
                                       .ForEach(x =>
                                       {
                                           parameters += string.Concat(x.Name, ": ", customClass.Name, ".", GetParameterName(codeMemberMethod.Name, x.Name));
                                       });

            return parameters += ");";
        }

        public void SaveAssembly(ref CodeNamespace customNamespace, string nameSpace, string className, string path, string[] referencedAssemblies)
        {
            //Create a new object of the global CodeCompileUnit class.
            CodeCompileUnit customAssembly = new CodeCompileUnit();

            //Add the namespace to the assembly.
            customAssembly.Namespaces.Add(customNamespace);

            //Add the following compiler parameters.  (The references to the //standard .net dll(s) and framework library).
            CompilerParameters comparam = new CompilerParameters(new string[] { "mscorlib.dll" });
            comparam.ReferencedAssemblies.Add("System.dll");
            comparam.ReferencedAssemblies.AddRange(referencedAssemblies);

            //Indicates Whether the compiler has to generate the output in //memory
            comparam.GenerateInMemory = false;

            //Indicates whether the output is an executable.
            comparam.GenerateExecutable = true;

            //provide the name of the class which contains the Main Entry //point method
            comparam.MainClass = string.Concat(nameSpace, ".", className);

            //provide the path where the generated assembly would be placed	
            comparam.OutputAssembly = Path.Combine(path, string.Concat(nameSpace, ".exe"));

            referencedAssemblies.ToList()
                                .ForEach(file => 
                                {
                                    File.Copy(file, Path.Combine(path, Path.GetFileName(file)), true);
                                });
            
            //Create an instance of the c# compiler and pass the assembly to //compile
            Microsoft.CSharp.CSharpCodeProvider ccp = new Microsoft.CSharp.CSharpCodeProvider();
            ICodeCompiler icc = ccp.CreateCompiler();

            //The CompileAssemblyFromDom would either return the list of 
            //compile time errors (if any), or would create the 
            //assembly in the respective path in case of successful //compilation
            CompilerResults compres = icc.CompileAssemblyFromDom(comparam, customAssembly);

            if (compres == null || compres.Errors.Count > 0)
            {
                for (int i = 0; i < compres.Errors.Count; i++)
                {
                    Console.WriteLine(compres.Errors[i]);
                }
            }

        }
    }
}
