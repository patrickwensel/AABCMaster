using System.Linq;
using AABC.ATrack.Integrators.ProviderApp;
using ATrack.Integrators.ProviderApp.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AABC.ATrack.Tests.Unit.Integrators
{
	[TestClass]
	public class ProviderApp
	{
		[TestMethod]
		public void GetSessionReportConfiguration()
		{
			IIntegrator integrator = new Integrator(null, null);

			var response = integrator.GetSessionReportConfiguration(null);

			Assert.IsNotNull(response);
			Assert.AreEqual(4, response.Barriers.Options.Count());
			Assert.AreEqual(7, response.Behaviors.Options.Count());
			Assert.AreEqual(3, response.Interventions.Options.Count());
			Assert.AreEqual(4, response.Reinforcers.Options.Count());
			Assert.IsNotNull(response.Goals);
		}


		//[TestMethod]
		//public void ConvertSessionReport()
		//{
		//	var integrator = new Integrator(null, null);

		//	var response = integrator.GetSessionReportConfiguration(null);

		//	response.Reinforcers.Options.First().Answer = true;
		//	response.Reinforcers.Options.First().Notes = "The notes for the first Reinforcer";
		//	response.Reinforcers.Options.Last().Answer = true;

		//	response.Barriers.Options.First().Answer = true;
		//	response.Barriers.Options.First().Notes = "The notes for the first Barrier";
		//	response.Barriers.Options.Last().Answer = true;

		//	response.Behaviors.Options.First().Answer = true;
		//	response.Behaviors.Options.First().Notes = "The notes for the first Behavior";
		//	response.Behaviors.Options.Last().Answer = true;

		//	response.Interventions.Options.First().Answer = response.Interventions.Options.First().Responses.First();
		//	response.Interventions.Options.First().Notes = "The notes for the first Intervention";
		//	response.Interventions.Options.Last().Answer = response.Interventions.Options.Last().Responses.First();

		//	response.Goals.Answer = "Goal Details";
		//	response.Goals.Progress = "Goal Progress";

		//	response.Summary.Answer = "This is the summary";

		//	DomainServices.Sessions.SessionReport sessionReport = integrator.ConvertSessionReport(response);

		//	Assert.IsNotNull(sessionReport);
		//	Assert.AreEqual(2, sessionReport.BarriersSection.Barriers.Count());
		//	Assert.AreEqual(2, sessionReport.BehaviorsSection.Behaviors.Count());
		//	Assert.AreEqual(2, sessionReport.InterventionsSection.Interventions.Count());
		//	Assert.AreEqual(2, sessionReport.ReinforcersSection.Reinforcers.Count());

		//}
	}
}
