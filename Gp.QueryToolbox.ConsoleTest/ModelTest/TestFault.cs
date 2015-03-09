using System;

namespace Gp.QueryToolbox.ConsoleTest.ModelTest
{
	public class TestFault
	{
		public Guid Id { get; set; }
		public string FaultCode { get; set; }
		public string Application { get; set; }
		public string Description { get; set; }
		public string FaultType { get; set; }

		public TestFault() {
			this.Id = Guid.NewGuid();
		}
	}
}
