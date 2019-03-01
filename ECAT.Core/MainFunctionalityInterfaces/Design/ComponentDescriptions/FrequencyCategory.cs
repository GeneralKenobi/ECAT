namespace ECAT.Core
{
	/// <summary>
	/// Enum for different categories of frequency description - each category is distinch and, because of that, is usually treated differently 
	/// </summary>
	public enum FrequencyCategory
	{
		/// <summary>
		/// Constant - frequency = 0
		/// </summary>
		DC = 0,

		/// <summary>
		/// Variable - frequency > 0
		/// </summary>
		AC = 1,
	}
}