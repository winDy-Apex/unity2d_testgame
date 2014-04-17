using System;

namespace GBaaS.io.Utils
{
	public abstract class Singleton<DerivedType>
		where DerivedType : new()
	{
		private static DerivedType _instance;

		public static DerivedType Instance
		{
			get
			{
				if (_instance == null) {
					_instance = new DerivedType ();
				}

				return _instance;
			}
		}
	}
}
