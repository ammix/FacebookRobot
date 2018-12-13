namespace FacebookRobot.Writers
{
	public interface IWriter
	{
		void Write(params string[] columnsData);
	}
}
