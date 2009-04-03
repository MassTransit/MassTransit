// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Tests.Serialization
{
	using System.Collections.Generic;
	using NUnit.Framework;

	[TestFixture]
	public class MoreSerialization_Specs :
		SerializationSpecificationBase
	{
		public class ContainerClass
		{
			public IList<OuterClass> Elements { get; set; }

			public bool Equals(ContainerClass other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;

				if (ReferenceEquals(other.Elements, Elements)) return true;
				if (other.Elements == null && Elements != null) return false;
				if (other.Elements != null && Elements == null) return false;

				if (other.Elements != null && Elements != null)
				{
					if (other.Elements.Count != Elements.Count) return false;

					for (int i = 0; i < Elements.Count; i++)
					{
						if (!Equals(other.Elements[i], Elements[i]))
							return false;
					}
				}

				return true;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof (ContainerClass)) return false;
				return Equals((ContainerClass) obj);
			}

			public override int GetHashCode()
			{
				return (Elements != null ? Elements.GetHashCode() : 0);
			}
		}

		public class OuterClass
		{
			public InnerClass Inner { get; set; }

			public bool Equals(OuterClass other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return Equals(other.Inner, Inner);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof (OuterClass)) return false;
				return Equals((OuterClass) obj);
			}

			public override int GetHashCode()
			{
				return (Inner != null ? Inner.GetHashCode() : 0);
			}
		}

		public class InnerClass
		{
			public string Name { get; set; }

			public bool Equals(InnerClass other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return Equals(other.Name, Name);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof (InnerClass)) return false;
				return Equals((InnerClass) obj);
			}

			public override int GetHashCode()
			{
				return (Name != null ? Name.GetHashCode() : 0);
			}
		}

		[Test]
		public void A_collection_of_objects_should_be_properly_serialized()
		{
			ContainerClass message = new ContainerClass
				{
					Elements = new List<OuterClass>
						{
							new OuterClass
								{
									Inner = new InnerClass {Name = "Chris"},
								},
							new OuterClass
								{
									Inner = new InnerClass {Name = "David"},
								},
						}
				};

			TestSerialization(message);
		}

		[Test]
		public void A_nested_object_should_be_properly_serialized()
		{
			OuterClass message = new OuterClass
				{
					Inner = new InnerClass {Name = "Chris"},
				};

			TestSerialization(message);
		}
	}
}