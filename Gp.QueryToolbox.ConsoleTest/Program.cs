using Gp.QueryToolbox.ConsoleTest.ModelTest;
using Gp.QueryToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Gp.QueryToolbox.ConsoleTest
{
	class Program
	{
		static void Main(string[] args) {
			// Initialisation des données de test
			var datas = InitializeDatas();

			// initialisation de l'objet SearchStatement à partir du fichier XML
			//var elem = XElement.Load(@".\search-1.xml");
			//var query = new Query(elem);Create a query from XML

			var grp = new AndGroup();
			grp.AddCriteria(new Criteria { Property = "FaultCode", Operator = Op.Contains, Value = "GEFCO" });
			grp.AddCriteria(new Criteria { Property = "Application", Operator = Op.Equals, Value = "DoIT.GAFI" });

			var grp2 = new AndGroup();
			grp2.AddCriteria(new Criteria { Property = "Application", Operator = Op.Equals, Value = "LogAuto.B2B.GEFCO" });

			var query = new Query();
			query.AddAndGroup(grp);
			query.AddAndGroup(grp2);

			var xml = QueryHelper.GetXElement(query).ToString();

			// construction de l'expression
			var queryResult = QueryHelper.GetExpression(datas.AsQueryable(), query).ToList();

			Console.WriteLine(query.ToString());
			Console.WriteLine(string.Format("{0} lignes trouvées : ", queryResult.Count));

			foreach (var result in queryResult) {
				Console.WriteLine(result.Description);
			}

			Console.ReadKey();
		}

		private static List<TestFault> InitializeDatas() {
			var result = new List<TestFault>();

			// initialisation de 10 erreurs dont 8 concernant GEFCO
			result.Add(new TestFault { FaultCode = "GEFCO", Application = "DoIT.GAFI", FaultType = "TRANSCO", Description="Erreur transco GEFCO 1" });
			result.Add(new TestFault { FaultCode = "GEFCO", Application = "DoIT.GAFI", FaultType = "TRANSCO", Description = "Erreur transco GEFCO 2" });
			result.Add(new TestFault { FaultCode = "GEFCO", Application = "DoIT.GAFI", FaultType = "TRANSCO", Description = "Erreur transco GEFCO 3" });
			result.Add(new TestFault { FaultCode = "XXXXX", Application = "LogAuto.B2B.GEFCO", FaultType = "TECH", Description = "Erreur technique GEFCO" });
			result.Add(new TestFault { FaultCode = "XXXXX", Application = "LogAuto.B2B.GEFCO", FaultType = "FONC", Description = "Erreur fonctionnelle GEFCO" });
			result.Add(new TestFault { FaultCode = "ecar-GEFCO", Application = "DoIT.GAFI", FaultType = "FONC", Description = "Erreur eCar GEFCO" });
			result.Add(new TestFault { FaultCode = "GEFCO_E", Application = "DoIT.GAFI", FaultType = "TRANSCO", Description = "Erreur transco GEFCO_E" });
			result.Add(new TestFault { FaultCode = "PH_R_ANN#GEFCO", Application = "DoIT.GAFI", FaultType = "TRANSCO", Description = "Erreur transco PHENIX pour GEFCO" });
			result.Add(new TestFault { FaultCode = "HYUNDAI", Application = "DoIT.GAFI", FaultType = "TRANSCO", Description = "Erreur transco HYUNDAI" });
			result.Add(new TestFault { FaultCode = "YYYYY", Application = "LogAuto.B2B.HYUNDAI", FaultType = "TECH", Description = "Erreur technique HYUNDAI" });			

			return result;
		}
	}
}
