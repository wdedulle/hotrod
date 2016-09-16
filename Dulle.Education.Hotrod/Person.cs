using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Dulle.Education.Hotrod
{
    internal class Person : IMessage<Person>
    {
        public int Id;
        public string FirstName;
        public string Surname;
        public int Age;
        private static readonly MessageParser<Person> _parser = new MessageParser<Person>(() => new Person());

        public static MessageParser<Person> Parser { get { return _parser; } }

        public Person()
        {
            this.Id = -1;
            this.FirstName = "";
            this.Surname = "";
            this.Age = -1;
        }

        public MessageDescriptor Descriptor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int CalculateSize()
        {
            int num = 0;
            if (this.Id > 0)
            {
                num += 1 + CodedOutputStream.ComputeInt32Size(this.Id);
            }
            if (this.Age > 0)
            {
                num += 1 + CodedOutputStream.ComputeInt32Size(this.Age);
            }
            if (this.FirstName.Length > 0)
            {
                num += 1 + CodedOutputStream.ComputeStringSize(this.FirstName);
            }
            if (this.Surname.Length > 0)
            {
                num += 1 + CodedOutputStream.ComputeStringSize(this.Surname);
            }

            return num;

        }



        public Person Clone()
        {
            throw new NotImplementedException();
        }

        public bool Equals(Person other)
        {
            if (other == null)
            {
                return false;
            }
            if (other != this)
            {
                if (this.Id != other.Id)
                {
                    return false;
                }
                if (this.Age != other.Age)
                {
                    return false;
                }
                if (this.FirstName != other.FirstName)
                {
                    return false;
                }
                if (this.Surname != other.Surname)
                {
                    return false;
                }
            }
            return true;

        }

        public void MergeFrom(CodedInputStream input)
        {
            uint num;
            while ((num = input.ReadTag()) > 0)
            {
                switch (num)
                {
                    case 34:
                        this.FirstName = input.ReadString();
                        break;

                    case 26:
                        this.Surname = input.ReadString();
                        break;

                    case 8:
                        this.Id = input.ReadInt32();
                        break;
                    case 16:
                        this.Age = input.ReadInt32();
                        break;

                    default:
                        input.SkipLastField();
                        break;
                }
            }
        }

        public void MergeFrom(Person other)
        {
            if (other != null)
            {
                if (other.Id > 0)
                {
                    this.Id = other.Id;
                }
                if (other.Age > 0)
                {
                    this.Age = other.Age;
                }
                if (other.Surname.Length > 0)
                {
                    this.Surname = other.Surname;
                }
                if (other.FirstName.Length > 0)
                {
                    this.FirstName = other.FirstName;
                }
            }
        }

        public void WriteTo(CodedOutputStream output)
        {
            if (this.Id > 0)
            {
                output.WriteRawTag(8);
                output.WriteInt32(this.Id);
            }
            if (this.Age > 0)
            {
                output.WriteRawTag(16);
                output.WriteInt32(this.Age);

            }

            if (this.Surname.Length > 0)
            {
                output.WriteRawTag(26);
                output.WriteString(this.Surname);
            }

            if (this.FirstName.Length > 0)
            {
                output.WriteRawTag(34);
                output.WriteString(this.FirstName);
            }


        }

        public override string ToString()
        {
            return "#####################\n" +
                  "User ID: " + this.Id + "\n" +
                  "Firstname: " + this.FirstName + "\n" +
                  "Surname: " + this.Surname + "\n" +
                  "Age: " + this.Age + "\n" +
                  "#####################\n"
                  ;
        }

    }
}