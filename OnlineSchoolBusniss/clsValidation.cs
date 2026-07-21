using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSchoolBusniss
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class StringPropertyLength : Attribute
    {
        public int MaxLength { get; }
        public int MinLength { get; }


        public StringPropertyLength(int min,int max)
        {
            MaxLength = max;
            MinLength = min;
        }
    }


    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class PersonValidate : Attribute
    {
        public PersonValidate()
        {
        }

    }


    public class clsValidation
    {
        public static bool PersonIsValide(clsPerson person) {
            Type type = person.GetType();

            foreach (var property in type.GetProperties()) {
                if (Attribute.IsDefined(property, typeof(StringPropertyLength)))
                {
                    var LengthAttribute=(StringPropertyLength)Attribute.GetCustomAttribute(property, typeof(StringPropertyLength));
                    int value = (property.GetValue(person).ToString()).Length;

                    if ( value<=LengthAttribute.MinLength || value > LengthAttribute.MaxLength) {

                        return false;
                    }

                }
                
                
            
            
            }


            return true;
        
        
        }

        public static bool StudentIsValid(clsStudent student) {
            Type type = student.GetType();

            foreach (var property in type.GetProperties())
            {   //this to verify if the personal info
                if (Attribute.IsDefined(property, typeof(PersonValidate)))
                {
                    clsPerson value = (clsPerson)property.GetValue(student);

                    if (!PersonIsValide(value))
                    {
                        return false;
                    }

                }
                //if we need the verification of some other property inside student 
                //we will just define the custom attribut for it and then add check it here
                if (Attribute.IsDefined(property, typeof(StringPropertyLength)))
                {
                    var LengthAttribute = (StringPropertyLength)Attribute.GetCustomAttribute(property, typeof(StringPropertyLength));
                    int value = (property.GetValue(student).ToString()).Length;

                    if (value <= LengthAttribute.MinLength || value > LengthAttribute.MaxLength)
                    {

                        return false;
                    }

                }


            }


            return true;

        }

        public static bool TeacherIsValid(clsTeacher Teacher)
        {
            Type type = Teacher.GetType();

            foreach (var property in type.GetProperties())
            {   //this to verify if the personal info
                if (Attribute.IsDefined(property, typeof(PersonValidate)))
                {
                    clsPerson value = (clsPerson)property.GetValue(Teacher);

                    if (!PersonIsValide(value))
                    {
                        return false;
                    }

                }
                //if we need the verification of some other property inside student 
                //we will just define the custom attribut for it and then add check it here
                if (Attribute.IsDefined(property, typeof(StringPropertyLength)))
                {
                    var LengthAttribute = (StringPropertyLength)Attribute.GetCustomAttribute(property, typeof(StringPropertyLength));
                    int value = (property.GetValue(Teacher).ToString()).Length;

                    if (value <= LengthAttribute.MinLength || value > LengthAttribute.MaxLength)
                    {

                        return false;
                    }

                }


            }


            return true;

        }

        public static bool CourseIsValide(clsCourse course)
        {

            Type type = course.GetType();

            foreach (var property in type.GetProperties())
            {   
                //checking the length of string properties
                if (Attribute.IsDefined(property, typeof(StringPropertyLength)))
                {
                    var LengthAttribute = (StringPropertyLength)Attribute.GetCustomAttribute(property, typeof(StringPropertyLength));
                    int value = (property.GetValue(course).ToString()).Length;

                    if (value <= LengthAttribute.MinLength || value > LengthAttribute.MaxLength)
                    {
                        return false;
                    }

                }





            }


            return true;
        }

        public static bool ContentIsValide(clsCourseContent Content)
        {

            Type type = Content.GetType();

            foreach (var property in type.GetProperties())
            {
                //checking the length of string properties
                if (Attribute.IsDefined(property, typeof(StringPropertyLength)))
                {
                    var LengthAttribute = (StringPropertyLength)Attribute.GetCustomAttribute(property, typeof(StringPropertyLength));
                    int value = (property.GetValue(Content).ToString()).Length;

                    if (value <= LengthAttribute.MinLength || value > LengthAttribute.MaxLength)
                    {
                        return false;
                    }

                }





            }


            return true;
        }

    }
}
