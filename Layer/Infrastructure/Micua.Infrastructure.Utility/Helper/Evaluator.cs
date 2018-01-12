// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Evaluator.cs" company="Wedn.Net">
//   Copyright ? 2014 Wedn.Net. All Rights Reserved.
// </copyright>
// <summary>
//   ��ִ̬����
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Micua.Infrastructure.Utility
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using Microsoft.CSharp;

    /// <summary> 
    /// ��ִ̬����
    /// </summary> 
    public class Evaluator
    {
        #region ˽�г�Ա
        /// <summary> 
        /// ���ڶ�̬�������ɵ��ִ࣬�����ڲ������Ŀ�ִ���ַ��� 
        /// </summary> 
        object _compiled;
        #endregion

        #region ���캯��
        /// <summary> 
        /// ��ִ�д��Ĺ��캯�� 
        /// </summary> 
        /// <param name="items"> 
        /// ��ִ���ַ������� 
        /// </param> 
        public Evaluator(IEnumerable<EvaluatorItem> items)
        {
            this.ConstructEvaluator(items);      // ���ý����ַ������캯�����н��� 
        }

        /// <summary> 
        /// ��ִ�д��Ĺ��캯�� 
        /// </summary> 
        /// <param name="returnType">����ֵ����</param> 
        /// <param name="expression">ִ�б��ʽ</param> 
        /// <param name="name">ִ���ַ�������</param> 
        public Evaluator(Type returnType, string expression, string name)
        {
            // ������ִ���ַ������� 
            EvaluatorItem[] items = { new EvaluatorItem(returnType, expression, name) };
            this.ConstructEvaluator(items);      // ���ý����ַ������캯�����н��� 
        }

        /// <summary> 
        /// ��ִ�д��Ĺ��캯�� 
        /// </summary> 
        /// <param name="item">��ִ���ַ�����</param> 
        public Evaluator(EvaluatorItem item)
        {
            EvaluatorItem[] items = { item }; // ����ִ���ַ�����תΪ��ִ���ַ��������� 
            this.ConstructEvaluator(items);      // ���ý����ַ������캯�����н��� 
        }

        /// <summary> 
        /// �����ַ������캯�� 
        /// </summary> 
        /// <param name="items">�������ַ�������</param> 
        private void ConstructEvaluator(IEnumerable<EvaluatorItem> items)
        {
            var provider = new CSharpCodeProvider();

            // ����C#������ʵ�� 
            ICodeCompiler comp = provider.CreateCompiler();

            // �������Ĵ������ 
            var cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");              // ��ӳ��� system.dll ������ 
            cp.ReferencedAssemblies.Add("system.data.dll");         // ��ӳ��� system.data.dll ������ 
            cp.ReferencedAssemblies.Add("system.xml.dll");          // ��ӳ��� system.xml.dll ������ 
            cp.GenerateExecutable = false;                          // �����ɿ�ִ���ļ� 
            cp.GenerateInMemory = true;                             // ���ڴ������� 

            var code = new StringBuilder();               // �������봮 

            // ��ӳ����ұ���������ַ���
            code.Append("using System; \n");
            code.Append("using System.Data; \n");
            code.Append("using System.Data.SqlClient; \n");
            code.Append("using System.Data.OleDb; \n");
            code.Append("using System.Xml; \n");

            code.Append("namespace Micua.Temp { \n");                  // ���ɴ���������ռ�ΪEvalGuy���ͱ�����һ�� 

            code.Append(" public class _Evaluator { \n");          // ���� _Evaluator �࣬���п�ִ�д�����ڴ��������� 
            // ����ÿһ����ִ���ַ����� 
            foreach (EvaluatorItem item in items)
            {   
                // ��Ӷ��幫���������� 
                // ��������ֵΪ��ִ���ַ������ж���ķ���ֵ���� 
                // ��������Ϊ��ִ���ַ������ж����ִ���ַ������� 
                code.AppendFormat("    public {0} {1}(object obj) ", item.ReturnType.Name, item.Name);

                code.Append("{ ");                                  // ��Ӻ�����ʼ���� 
                // code.AppendFormat("return ({0});", item.Expression);//��Ӻ����壬���ؿ�ִ���ַ������ж���ı��ʽ��ֵ
                code.AppendFormat(item.Expression); // ��Ӻ����壬���ؿ�ִ���ַ������ж���ı��ʽ��ֵ
                code.Append("}\n"); // ��Ӻ����������� 
            }

            code.Append("} }");                                 // ���������������ռ�������� 

            // �õ�������ʵ���ķ��ؽ�� 
            CompilerResults cr = comp.CompileAssemblyFromSource(cp, code.ToString());

            // ����д��� 
            if (cr.Errors.HasErrors)                            
            {
                var error = new StringBuilder();          // ����������Ϣ�ַ��� 
                error.Append("�����д���ı��ʽ: ");                // ��Ӵ����ı� 
                // ����ÿһ�����ֵı������ 
                foreach (CompilerError err in cr.Errors)
                {
                    // ��ӽ������ı���ÿ��������� 
                    error.AppendFormat("{0}\n", err.ErrorText);
                }

                throw new Exception("�������: " + error); // �׳��쳣 
            }

            Assembly a = cr.CompiledAssembly;                       // ��ȡ������ʵ���ĳ��� 
            this._compiled = a.CreateInstance("Micua.Temp._Evaluator");     // ͨ�����򼯲��Ҳ����� EvalGuy._Evaluator ��ʵ�� 
        }
        #endregion

        #region ���г�Ա

        /// <summary> 
        /// ִ���ַ�������������ֵ 
        /// </summary> 
        /// <param name="name">ִ���ַ�������</param>
        /// <param name="obj">��������</param>
        /// <returns>ִ�н��</returns> 
        public int EvaluateInt(string name, object obj)
        {
            return (int)this.Evaluate(name, obj);
        }

        /// <summary> 
        /// ִ���ַ����������ַ�����ֵ 
        /// </summary> 
        /// <param name="name">ִ���ַ�������</param> 
        /// <param name="obj">��������</param>
        /// <returns>ִ�н��</returns> 
        public string EvaluateString(string name, object obj)
        {
            return (string)this.Evaluate(name, obj);
        }

        /// <summary> 
        /// ִ���ַ��������ز�����ֵ 
        /// </summary> 
        /// <param name="name">ִ���ַ�������</param> 
        /// <param name="obj">��������</param>
        /// <returns>ִ�н��</returns> 
        public bool EvaluateBool(string name, object obj)
        {
            return (bool)this.Evaluate(name, obj);
        }

        /// <summary> 
        /// ִ���ַ������� object ��ֵ 
        /// </summary> 
        /// <param name="name">ִ���ַ�������</param> 
        /// <param name="obj">��������</param>
        /// <returns>ִ�н��</returns> 
        public object Evaluate(string name, object obj)
        {
            MethodInfo mi = this._compiled.GetType().GetMethod(name); // ��ȡ _Compiled ��������������Ϊ name �ķ��������� 
            object[] objs = { obj };
            return mi.Invoke(this._compiled, objs); // ִ�� mi �����õķ���
        }
        #endregion
    }
}