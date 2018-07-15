using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Cerval_Library
{
    [Serializable]
    public class Untis_simple_element
    {
        [XmlAttribute]
        public string id;
    }

    [Serializable]
    public class Untis_general
    {
        public string schoolname;
        public string schoolyearbegindate;
        public string schoolyearenddate;
        public string header1;
        public string header2;
        public string footer;
    }

    [Serializable]
    public class Untis_timeperiod
    {
        [XmlAttribute]
        public string id;
        public string day;
        public string period;
        public string starttime;
        public string endtime;
        public int TT_column;
    }
    [Serializable]
    public class Untis_room
    {
        [XmlAttribute]
        public string id;
        public string longname;
        public string text;
        public int TT_code;
        public string capacity;
    }

    [Serializable]
    public class Untis_teacher
    {
        [XmlAttribute]
        public string id;
        public string surname;
        public string forename;
        public string gender;
        public string text;
        public Untis_simple_element teacher_department;
        public Untis_simple_element teacher_room;
        public int TT_staffindex;

        public Untis_teacher()
        {
            teacher_department = new Untis_simple_element();
        }
        public Untis_teacher(string code, string Surname, string Forename)
        {
            teacher_department = new Untis_simple_element(); teacher_department.id = "";
            id = "TR_" + code; surname = Surname; forename = Forename;
        }
    }
    [Serializable]
    public class Untis_subject
    {
        [XmlAttribute]
        public string id;
        public string longname;
        public string subjectgroup;
        public string subject_room;
    }

    [Serializable]
    public class Untis_department
    {
        [XmlAttribute] public string id;
    }


    [Serializable]  
    public class Untis_class
    {
        [XmlAttribute]
        public string id;
        public string longname;
        public string classlevel;
        public string studentsmale;
        public string studentsfemale;
        public Untis_simple_element class_room;
        public Untis_class()
        {
            class_room = new Untis_simple_element();
        }
    }


    [Serializable]
    public class  Untis_time
    {
        public string assigned_day;
        public string assigned_period;
        public Untis_simple_element assigned_room;
        public Untis_time()
        {
            assigned_room = new Untis_simple_element();
        }
        public Untis_time(int day, int period, string roomid)
        {
            assigned_room = new Untis_simple_element(); assigned_room.id = roomid;
            assigned_day = day.ToString();
            assigned_period = period.ToString();
        }
    }
    [Serializable]
    public class Untis_lesson
    {
        [XmlAttribute]
        public string id;
        public int periods;
        public Untis_simple_element lesson_subject;
        public Untis_simple_element lesson_teacher;
        public Untis_simple_element lesson_classes;
        public Untis_simple_element lesson_students;
        public Untis_time[] times;
        public string text1;
        public string text2;
        public string NovaName;

        public Untis_lesson()
        {
            lesson_subject = new Untis_simple_element();
            lesson_teacher = new Untis_simple_element();
            lesson_classes = new Untis_simple_element();
            lesson_students = new Untis_simple_element();
            text1 = ""; text2 = "";
        }
    }

    [Serializable]
    [XmlInclude(typeof(Untis_lesson)), XmlInclude(typeof(Untis_time)), XmlInclude(typeof(Untis_general)),XmlInclude(typeof(Untis_class))]
    public class Untis
    {

        public Untis_general general;
        public Untis_timeperiod[] timeperiods;
        public Untis_department[] departments;
        public Untis_room[] rooms;
        public Untis_teacher[] teachers;
        public Untis_subject[] subjects;
        public Untis_class[] classes;
        public Untis_lesson[] lessons;

        public Untis()
        {
        }
        public Untis(int no_periods, int no_rooms,int no_teachers,int no_subjects, int no_classes,int no_lessons)
        {
            general = new Untis_general();
            timeperiods = new Untis_timeperiod[no_periods];
            departments = new Untis_department[100];
            rooms = new Untis_room[no_rooms];
            teachers = new Untis_teacher[no_teachers];
            subjects = new Untis_subject[no_subjects];
            classes = new Untis_class[no_classes];
            lessons = new Untis_lesson[no_lessons];
            for (int i = 0; i < no_periods; i++) { timeperiods[i] = new Untis_timeperiod(); }
            for (int i = 0; i < no_rooms; i++) { rooms[i] = new Untis_room(); }
            for (int i = 0; i < no_teachers; i++) { teachers[i] = new Untis_teacher(); }
            for (int i = 0; i < no_subjects; i++) { subjects[i] = new Untis_subject(); }
            for (int i = 0; i < no_classes; i++) { classes[i] = new Untis_class(); }
        }

        public void MakeSetNames()
        {
            string s1 = ""; string s2 = "";
            foreach (Untis_lesson l1 in lessons)
            {
                //if we have text1 use it...
                if (l1.text1 != "") l1.NovaName = l1.text1;
                else
                {
                    s2 = l1.lesson_classes.id.Substring(3); s1 = l1.lesson_subject.id.Substring(3);
                    if ((s1.StartsWith("7")) || (s1.StartsWith("8")) || (s1.StartsWith("9")) || (s1.StartsWith("10"))) l1.NovaName = s1; else l1.NovaName = s2;
                }
            }
        }
    }
}
