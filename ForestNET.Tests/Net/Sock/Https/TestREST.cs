namespace ForestNET.Tests.Net.Sock.Https
{
    public class TestREST : ForestNET.Lib.Net.Https.REST.ForestREST
    {

        /* Fields */

        private readonly List<Person> a_persons = [];
        private List<Message> a_messages = [];

        /* Properties */

        /* Methods */

        public TestREST()
        {
            this.a_persons.Add(new Person(1, 643532, "John Smith", 32, "New York", "US"));
            this.a_persons.Add(new Person(2, 284255, "Elizabeth Miller", 21, "Hamburg", "DE"));
            this.a_persons.Add(new Person(3, 116974, "Jennifer Garcia", 48, "London", "UK"));
            this.a_persons.Add(new Person(4, 295556, "Jakub Kowalski", 39, "Warsaw", "PL"));

            this.a_messages.Add(new Message(1, 1, 4, "Subject #1", "Message #1"));
            this.a_messages.Add(new Message(1, 4, 3, "Subject #2", "Message #2"));
            this.a_messages.Add(new Message(1, 2, 1, "Subject #3", "Message #3"));
            this.a_messages.Add(new Message(2, 4, 1, "Subject #4", "Message #4"));
            this.a_messages.Add(new Message(2, 2, 4, "Subject #5", "Message #5"));
            this.a_messages.Add(new Message(2, 2, 3, "Subject #6", "Message #6"));
            this.a_messages.Add(new Message(3, 4, 3, "Subject #7", "Message #7"));
            this.a_messages.Add(new Message(1, 3, 2, "Subject #8", "Message #8"));
        }

        public override string HandleGET()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new InvalidDataException("Seed instance is not available");
            }

            if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.RequestHeader.Path))
            {
                if (this.Seed.RequestHeader.File.Equals("persons"))
                {
                    string s_foo = "";

                    foreach (Person o_person in this.a_persons)
                    {
                        if (this.SkipPersonRecord(o_person))
                        {
                            continue;
                        }

                        s_foo += o_person.ToString() + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
                    }

                    if (s_foo.Length >= ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK.Length)
                    {
                        s_foo = s_foo.Substring(0, s_foo.Length - ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK.Length);
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(s_foo))
                    {
                        return "No results.";
                    }

                    return s_foo;
                }
                else if (this.Seed.RequestHeader.File.Equals("messages"))
                {
                    string s_foo = "";

                    foreach (Message o_message in this.a_messages)
                    {
                        if (this.SkipMessageRecord(o_message))
                        {
                            continue;
                        }

                        s_foo += o_message.ToString(this.a_persons) + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
                    }

                    if (s_foo.Length >= ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK.Length)
                    {
                        s_foo = s_foo.Substring(0, s_foo.Length - ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK.Length);
                    }

                    if (ForestNET.Lib.Helper.IsStringEmpty(s_foo))
                    {
                        return "No results.";
                    }

                    return s_foo;
                }
                else
                {
                    return "400;Invalid request.";
                }
            }
            else
            {
                if ((this.Seed.RequestHeader.Path.Equals("persons")) && (ForestNET.Lib.Helper.IsInteger(this.Seed.RequestHeader.File)))
                {
                    foreach (Person o_person in this.a_persons)
                    {
                        if (o_person.ID == int.Parse(this.Seed.RequestHeader.File))
                        {
                            return o_person.ToString();
                        }
                    }

                    return "No results.";
                }
                else if ((this.Seed.RequestHeader.Path.Equals("messages")) && (ForestNET.Lib.Helper.IsInteger(this.Seed.RequestHeader.File)))
                {
                    foreach (Message o_message in this.a_messages)
                    {
                        if (o_message.ID == int.Parse(this.Seed.RequestHeader.File))
                        {
                            return o_message.ToString(this.a_persons);
                        }
                    }

                    return "No results.";
                }
                else if ((this.Seed.RequestHeader.RequestPath.Contains("persons")) && (this.Seed.RequestHeader.RequestPath.Contains("messages")))
                {
                    string[] a_path = this.Seed.RequestHeader.RequestPath.Substring(1).Split("/");

                    if ((a_path.Length == 4) && (a_path[0].Equals("persons")) && (ForestNET.Lib.Helper.IsInteger(a_path[1])) && (a_path[2].Equals("messages")) && (ForestNET.Lib.Helper.IsInteger(a_path[3])))
                    {
                        int i_personID = int.Parse(a_path[1]);
                        int i_messageID = int.Parse(a_path[3]);

                        foreach (Person o_person in this.a_persons)
                        {
                            if (o_person.ID == i_personID)
                            {
                                foreach (Message o_message in this.a_messages)
                                {
                                    if ((o_message.To == o_person.ID) && (o_message.ID == i_messageID))
                                    {
                                        return o_message.ToString(this.a_persons);
                                    }
                                }
                            }
                        }

                        return "No results.";
                    }
                    else if ((a_path.Length == 3) && (a_path[0].Equals("persons")) && (ForestNET.Lib.Helper.IsInteger(a_path[1])) && (a_path[2].StartsWith("messages")))
                    {
                        int i_personID = int.Parse(a_path[1]);
                        string s_foo = "";

                        foreach (Message o_message in this.a_messages)
                        {
                            if ((o_message.To != i_personID) || (this.SkipMessageRecord(o_message)))
                            {
                                continue;
                            }

                            s_foo += o_message.ToString(this.a_persons) + ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
                        }

                        if (s_foo.Length >= ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK.Length)
                        {
                            s_foo = s_foo.Substring(0, s_foo.Length - ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK.Length);
                        }

                        if (ForestNET.Lib.Helper.IsStringEmpty(s_foo))
                        {
                            return "No results.";
                        }

                        return s_foo;
                    }
                    else
                    {
                        return "400;Invalid request.";
                    }
                }
                else
                {
                    return "400;Invalid request.";
                }
            }
        }

        private bool SkipPersonRecord(Person p_o_person)
        {
            if (this.Seed == null)
            {
                return false;
            }

            bool b_skip = false;

            foreach (KeyValuePair<string, string?> o_paramPair in this.Seed.RequestHeader.Parameters)
            {
                b_skip = false;

                string s_key = o_paramPair.Key;
                string? s_value = o_paramPair.Value;
                string s_operator = "eq";

                if ((s_key.Contains('[')) && (s_key.Contains(']')))
                {
                    s_operator = s_key.Substring(s_key.IndexOf("[") + 1, s_key.IndexOf("]") - (s_key.IndexOf("[") + 1)).ToLower();
                    s_key = s_key.Substring(0, s_key.IndexOf("["));
                }

                bool b_exc = false;

                switch (s_operator)
                {
                    case "eq":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_person.ID != int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "PersonalIdentificationNumber":
                                if (p_o_person.PersonalIdentificationNumber != int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "Name":
                                if (!p_o_person.Name.Equals(s_value)) b_skip = true;
                                break;
                            case "Age":
                                if (p_o_person.Age != int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "City":
                                if (!p_o_person.City.Equals(s_value)) b_skip = true;
                                break;
                            case "Country":
                                if (!p_o_person.Country.Equals(s_value)) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "gt":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_person.ID <= int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "Age":
                                if (p_o_person.Age <= int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "gte":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_person.ID < int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "Age":
                                if (p_o_person.Age < int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "lt":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_person.ID >= int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "Age":
                                if (p_o_person.Age >= int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "lte":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_person.ID > int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "Age":
                                if (p_o_person.Age > int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "ne":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_person.ID == int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "PersonalIdentificationNumber":
                                if (p_o_person.PersonalIdentificationNumber == int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "Name":
                                if (p_o_person.Name.Equals(s_value)) b_skip = true;
                                break;
                            case "Age":
                                if (p_o_person.Age == int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "City":
                                if (p_o_person.City.Equals(s_value)) b_skip = true;
                                break;
                            case "Country":
                                if (p_o_person.Country.Equals(s_value)) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "starts":
                        switch (s_key)
                        {
                            case "Name":
                                if (!p_o_person.Name.StartsWith(s_value ?? " ")) b_skip = true;
                                break;
                            case "City":
                                if (!p_o_person.City.StartsWith(s_value ?? " ")) b_skip = true;
                                break;
                            case "Country":
                                if (!p_o_person.Country.StartsWith(s_value ?? " ")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "ends":
                        switch (s_key)
                        {
                            case "Name":
                                if (!p_o_person.Name.EndsWith(s_value ?? " ")) b_skip = true;
                                break;
                            case "City":
                                if (!p_o_person.City.EndsWith(s_value ?? " ")) b_skip = true;
                                break;
                            case "Country":
                                if (!p_o_person.Country.EndsWith(s_value ?? " ")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new Exception("Invalid request.");
                }

                if (b_skip)
                {
                    break;
                }
            }

            return b_skip;
        }

        private bool SkipMessageRecord(Message p_o_message)
        {
            if (this.Seed == null)
            {
                return false;
            }

            bool b_skip = false;

            foreach (KeyValuePair<string, string?> o_paramPair in this.Seed.RequestHeader.Parameters)
            {
                b_skip = false;

                string s_key = o_paramPair.Key;
                string? s_value = o_paramPair.Value;
                string s_operator = "eq";

                if ((s_key.Contains('[')) && (s_key.Contains(']')))
                {
                    s_operator = s_key.Substring(s_key.IndexOf("[") + 1, s_key.IndexOf("]") - (s_key.IndexOf("[") + 1)).ToLower();
                    s_key = s_key.Substring(0, s_key.IndexOf("["));
                }

                bool b_exc = false;

                switch (s_operator)
                {
                    case "eq":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_message.ID != int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "From":
                                if (p_o_message.From != int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "To":
                                if (p_o_message.To != int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "Subject":
                                if (!p_o_message.Subject.Equals(s_value)) b_skip = true;
                                break;
                            case "Message":
                                if (!p_o_message.MessageStr.Equals(s_value)) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "gt":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_message.ID <= int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "From":
                                if (p_o_message.From <= int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "To":
                                if (p_o_message.To <= int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "gte":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_message.ID < int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "From":
                                if (p_o_message.From < int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "To":
                                if (p_o_message.To < int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "lt":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_message.ID >= int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "From":
                                if (p_o_message.From >= int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "To":
                                if (p_o_message.To >= int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "lte":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_message.ID > int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "From":
                                if (p_o_message.From > int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "To":
                                if (p_o_message.To > int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "ne":
                        switch (s_key)
                        {
                            case "ID":
                                if (p_o_message.ID == int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "From":
                                if (p_o_message.From == int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "To":
                                if (p_o_message.To == int.Parse(s_value ?? "0")) b_skip = true;
                                break;
                            case "Subject":
                                if (p_o_message.Subject.Equals(s_value)) b_skip = true;
                                break;
                            case "Message":
                                if (p_o_message.MessageStr.Equals(s_value)) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "starts":
                        switch (s_key)
                        {
                            case "Subject":
                                if (!p_o_message.Subject.StartsWith(s_value ?? " ")) b_skip = true;
                                break;
                            case "Message":
                                if (!p_o_message.MessageStr.StartsWith(s_value ?? " ")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    case "ends":
                        switch (s_key)
                        {
                            case "Subject":
                                if (!p_o_message.Subject.EndsWith(s_value ?? " 0")) b_skip = true;
                                break;
                            case "Message":
                                if (!p_o_message.MessageStr.EndsWith(s_value ?? " ")) b_skip = true;
                                break;
                            default:
                                b_exc = true;
                                break;
                        }
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new Exception("Invalid request.");
                }

                if (b_skip)
                {
                    break;
                }
            }

            return b_skip;
        }

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
        public override string HandlePOST()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new InvalidDataException("Seed instance is not available");
            }

            if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.RequestHeader.Path))
            {
                if (this.Seed.RequestHeader.File.Equals("persons"))
                {
                    if (this.Seed.PostData.Count != 5)
                    {
                        return "400;Invalid request. Not enough input data [" + this.Seed.PostData.Count + "], need [PIN, Name, Age, City, Country].";
                    }


                    if ((!this.Seed.PostData.ContainsKey("PIN")) || (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["PIN"])))
                    {
                        return "400;Invalid request. No input data 'PIN'.";
                    }

                    if ((!this.Seed.PostData.ContainsKey("Name")) || (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["Name"])))
                    {
                        return "400;Invalid request. No input data 'Name'.";
                    }

                    if ((!this.Seed.PostData.ContainsKey("Age")) || (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["Age"])))
                    {
                        return "400;Invalid request. No input data 'Age'.";
                    }

                    if ((!this.Seed.PostData.ContainsKey("City")) || (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["City"])))
                    {
                        return "400;Invalid request. No input data 'City'.";
                    }

                    if ((!this.Seed.PostData.ContainsKey("Country")) || (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["Country"])))
                    {
                        return "400;Invalid request. No input data 'Country'.";
                    }

                    if (this.Seed.PostData["PIN"]?.Length != 6)
                    {
                        return "400;Invalid request. Input data 'PIN' must be a 6-digit integer.";
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(this.Seed.PostData["PIN"] ?? "0"))
                    {
                        return "400;Invalid request. Input data 'PIN' is not an integer.";
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(this.Seed.PostData["Age"] ?? "0"))
                    {
                        return "400;Invalid request. Input data 'Age' is not an integer.";
                    }

                    if (int.Parse(this.Seed.PostData["Age"] ?? "0") < 0)
                    {
                        return "400;Invalid request. Input data 'Age' must be a positive integer.";
                    }

                    if (this.Seed.PostData["Country"]?.Length != 2)
                    {
                        return "400;Invalid request. Input data 'Country' must be country code with length of '2'.";
                    }

                    int i_id = this.a_persons.Count + 1;

                    this.a_persons.Add(
                        new Person(
                            i_id,
                            int.Parse(this.Seed.PostData["PIN"] ?? "0"),
                            this.Seed.PostData["Name"] ?? string.Empty,
                            int.Parse(this.Seed.PostData["Age"] ?? "0"),
                            this.Seed.PostData["City"] ?? string.Empty,
                            this.Seed.PostData["Country"] ?? string.Empty
                        )
                    );

                    return this.Seed.RequestHeader.RequestPath + "/" + i_id;
                }
                else
                {
                    return "400;Invalid request.";
                }
            }
            else
            {
                string[] a_path = this.Seed.RequestHeader.RequestPath.Substring(1).Split("/");

                if ((a_path.Length == 3) && (a_path[0].Equals("persons")) && (ForestNET.Lib.Helper.IsInteger(a_path[1])) && (a_path[2].Equals("messages")))
                {
                    int i_fromId = int.Parse(a_path[1]);

                    if (this.Seed.PostData.Count != 3)
                    {
                        return "400;Invalid request. Not enough input data [" + this.Seed.PostData.Count + "], need [To, Subject, Message].";
                    }

                    if ((!this.Seed.PostData.ContainsKey("To")) || (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["To"])))
                    {
                        return "400;Invalid request. No input data 'To'.";
                    }

                    if ((!this.Seed.PostData.ContainsKey("Subject")) || (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["Subject"])))
                    {
                        return "400;Invalid request. No input data 'Subject'.";
                    }

                    if ((!this.Seed.PostData.ContainsKey("Message")) || (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["Message"])))
                    {
                        return "400;Invalid request. No input data 'Message'.";
                    }

                    int i_toID = 0;

                    foreach (Person o_person in this.a_persons)
                    {
                        if (o_person.Name.Equals(this.Seed.PostData["To"]))
                        {
                            i_toID = o_person.ID;
                        }
                    }

                    if (i_toID < 1)
                    {
                        return "400;Invalid request. Invalid input data 'To'. '" + this.Seed.PostData["To"] + "' not found.";
                    }

                    int i_id = 1;

                    foreach (Message o_message in this.a_messages)
                    {
                        if (o_message.To == i_toID)
                        {
                            i_id++;
                        }
                    }

                    this.a_messages.Add(
                        new Message(
                            i_id,
                            i_fromId,
                            i_toID,
                            this.Seed.PostData["Subject"] ?? string.Empty,
                            this.Seed.PostData["Message"] ?? string.Empty
                        )
                    );

                    return "/persons/" + i_toID + "/messages/" + i_id;
                }
                else
                {
                    return "400;Invalid request.";
                }
            }
        }

        public override string HandlePUT()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new InvalidDataException("Seed instance is not available");
            }

            string[] a_path = this.Seed.RequestHeader.RequestPath.Substring(1).Split("/");

            if ((a_path.Length == 2) && (a_path[0].Equals("persons")) && (ForestNET.Lib.Helper.IsInteger(a_path[1])))
            {
                int i_personID = int.Parse(a_path[1]) - 1;
                Person o_person = this.a_persons[i_personID];

                if (this.Seed.PostData.ContainsKey("PIN"))
                {
                    if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["PIN"]))
                    {
                        return "400;Invalid request. Input data 'PIN' for change is empty.";
                    }

                    if (this.Seed.PostData["PIN"]?.Length != 6)
                    {
                        return "400;Invalid request. Input data 'PIN' for change must be a 6-digit integer.";
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(this.Seed.PostData["PIN"] ?? "0"))
                    {
                        return "400;Invalid request. Input data 'PIN' for change is not an integer.";
                    }

                    o_person.PersonalIdentificationNumber = int.Parse(this.Seed.PostData["PIN"] ?? "0");
                }

                if (this.Seed.PostData.ContainsKey("Name"))
                {
                    if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["Name"]))
                    {
                        return "400;Invalid request. Input data 'Name' for change is empty.";
                    }

                    o_person.Name = this.Seed.PostData["Name"] ?? string.Empty;
                }

                if (this.Seed.PostData.ContainsKey("Age"))
                {
                    if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["Age"]))
                    {
                        return "400;Invalid request. Input data 'Age' for change is empty.";
                    }

                    if (!ForestNET.Lib.Helper.IsInteger(this.Seed.PostData["Age"] ?? "0"))
                    {
                        return "400;Invalid request. Input data 'Age' is not an integer.";
                    }

                    if (int.Parse(this.Seed.PostData["Age"] ?? "0") < 0)
                    {
                        return "400;Invalid request. Input data 'Age' must be a positive integer.";
                    }

                    o_person.Age = int.Parse(this.Seed.PostData["Age"] ?? "0");
                }

                if (this.Seed.PostData.ContainsKey("City"))
                {
                    if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["City"]))
                    {
                        return "400;Invalid request. Input data 'City' for change is empty.";
                    }

                    o_person.City = this.Seed.PostData["City"] ?? string.Empty;
                }

                if (this.Seed.PostData.ContainsKey("Country"))
                {
                    if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["Country"]))
                    {
                        return "400;Invalid request. Input data 'Country' for change is empty.";
                    }

                    if (this.Seed.PostData["Country"]?.Length != 2)
                    {
                        return "400;Invalid request. Input data 'Country' must be country code with length of '2'.";
                    }

                    o_person.Country = this.Seed.PostData["Country"] ?? string.Empty;
                }

                this.a_persons[i_personID] = o_person;

                return this.Seed.RequestHeader.RequestPath;
            }
            else if ((a_path.Length == 4) && (a_path[0].Equals("persons")) && (ForestNET.Lib.Helper.IsInteger(a_path[1])) && (a_path[2].Equals("messages")) && (ForestNET.Lib.Helper.IsInteger(a_path[3])))
            {
                int i_personID = int.Parse(a_path[1]);
                int i_messageID = int.Parse(a_path[3]);
                int i_foo = 0;
                Message? o_message = null;

                foreach (Message o_foo in this.a_messages)
                {
                    if ((o_foo.To == i_personID) && (o_foo.ID == i_messageID))
                    {
                        o_message = o_foo;
                        break;
                    }

                    i_foo++;
                }

                if (o_message == null)
                {
                    return "400;Invalid request. Message for change not found.";
                }

                if (this.Seed.PostData.ContainsKey("Subject"))
                {
                    if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["Subject"]))
                    {
                        return "400;Invalid request. Input data 'Subject' for change is empty.";
                    }

                    o_message.Subject = this.Seed.PostData["Subject"] ?? string.Empty;
                }

                if (this.Seed.PostData.ContainsKey("Message"))
                {
                    if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.PostData["Message"]))
                    {
                        return "400;Invalid request. Input data 'Message' for change is empty.";
                    }

                    o_message.MessageStr = this.Seed.PostData["Message"] ?? string.Empty;
                }

                this.a_messages[i_foo] = o_message;

                return this.Seed.RequestHeader.RequestPath;
            }
            else
            {
                return "400;Invalid request.";
            }
        }
#pragma warning restore CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
#pragma warning restore IDE0079 // Remove unnecessary suppression

        public override string HandleDELETE()

        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new InvalidDataException("Seed instance is not available");
            }

            string[] a_path = this.Seed.RequestHeader.RequestPath.Substring(1).Split("/");

            if ((a_path.Length == 2) && (a_path[0].Equals("persons")) && (ForestNET.Lib.Helper.IsInteger(a_path[1])))
            {
                int i_personID = int.Parse(a_path[1]);
                Person o_person = this.a_persons[i_personID - 1];

                this.a_persons.Remove(o_person);

                List<Message> a_foo = [];

                foreach (Message o_message in this.a_messages)
                {
                    if ((o_message.From != i_personID) && (o_message.To != i_personID))
                    {
                        a_foo.Add(o_message);
                    }
                }

                this.a_messages = a_foo;

                return "/persons";
            }
            else if ((a_path.Length == 4) && (a_path[0].Equals("persons")) && (ForestNET.Lib.Helper.IsInteger(a_path[1])) && (a_path[2].Equals("messages")) && (ForestNET.Lib.Helper.IsInteger(a_path[3])))
            {
                int i_personID = int.Parse(a_path[1]);
                int i_messageID = int.Parse(a_path[3]);
                Message? o_message = null;

                foreach (Message o_foo in this.a_messages)
                {
                    if ((o_foo.To == i_personID) && (o_foo.ID == i_messageID))
                    {
                        o_message = o_foo;
                        break;
                    }
                }

                if (o_message == null)
                {
                    return "400;Invalid request. Message for deletion not found.";
                }

                this.a_messages.Remove(o_message);

                return "/persons/" + i_personID + "/messages";
            }
            else
            {
                return "400;Invalid request.";
            }
        }

        /* Internal Classes */

        internal class Person
        {

            /* Fields */

            public int ID;
            public int PersonalIdentificationNumber;
            public string Name;
            public int Age;
            public string City;
            public string Country;

            /* Properties */

            /* Methods */

            public Person(int p_i_ID, int p_i_PIN, string p_s_name, int p_i_age, string p_s_city, string p_s_country)
            {
                this.ID = p_i_ID;
                this.PersonalIdentificationNumber = p_i_PIN;
                this.Name = p_s_name;
                this.Age = p_i_age;
                this.City = p_s_city;
                this.Country = p_s_country;
            }

            public override string ToString()
            {
                return this.ID + ";" + this.PersonalIdentificationNumber + ";" + this.Name + ";" + this.Age + ";" + this.City + ";" + this.Country;
            }
        }

        internal class Message
        {

            /* Fields */

            public int ID;
            public int From;
            public int To;
            public string Subject;
            public string MessageStr;

            /* Properties */

            /* Methods */

            public Message(int p_i_ID, int p_i_from, int p_i_to, string p_s_subject, string p_s_message)
            {
                this.ID = p_i_ID;
                this.From = p_i_from;
                this.To = p_i_to;
                this.Subject = p_s_subject;
                this.MessageStr = p_s_message;
            }

            public override string ToString()
            {
                return this.ToString(null);
            }

            public string ToString(List<Person>? p_a_persons)
            {
                if (p_a_persons == null)
                {
                    return "Person list parameter is null";
                }

                string s_from = Convert.ToString(this.From);
                string s_to = Convert.ToString(this.To);

                foreach (Person o_person in p_a_persons)
                {
                    if (o_person.ID == this.From)
                    {
                        s_from = o_person.Name;
                    }

                    if (o_person.ID == this.To)
                    {
                        s_to = o_person.Name;
                    }
                }

                return this.ID + ";" + s_from + ";" + s_to + ";" + this.Subject + ";" + this.MessageStr;
            }
        }
    }
}
