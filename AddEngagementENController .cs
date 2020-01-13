using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Engagewment.DataAccessLayer;
using Engagewment.Models;
using Engagewment.ViewModels;

namespace Engagewment.Controllers
{
    public class AddEngagementENController : Controller
    {
        private EngagementContext db = new EngagementContext();
        public ActionResult Index(int page, string sid)
        {
            int[] code = GetCode(sid);
            int x = 2;
            x = code[0];
            var engageid = code[1];
            var currentUrl = string.Empty;
            currentUrl = GetUrl(engageid);
            // var fullUrl = this.Url.Action("index", "AddEngagement",null, this.Request.Url.Scheme);
            if (currentUrl != string.Empty)
            {
                // currentUrl = fullUrl;
                return Redirect(currentUrl);
            }
            switch (x)
            {
                case 0://ยังไม่ได้ทำแบบทดสอบ 
                    break;

                case 1://ทำแบบทดสอบเรียบร้อยแล้ว

                    break;

                default://2 คือเลข Code ไม่ถูกต้อง
                    return HttpNotFound();

            }
            var viewModel = new ParameterValue()
            {
                StatusAccept = x,
                Encript_ID = sid,
                engagepersonal_ID = engageid
            };

            return View(viewModel);
        }
        public void SaveCurrentUrl(string fullUrl, int engageid)
        {
            TB_Url addUrl = new TB_Url();
            addUrl.engagepersonal_ID = engageid;
            addUrl.url = fullUrl;
            db.TB_Url.Add(addUrl);
            db.SaveChanges();
        }
        public void updateUrl(string fullUrl, int engageid)
        {

            var engagementUrl = db.TB_Url.Where(x => x.engagepersonal_ID == engageid).FirstOrDefault();
            if (engagementUrl != null)
            {
                engagementUrl.url = fullUrl;
                db.Entry(engagementUrl).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        public string GetUrl(int id)
        {
            var url = string.Empty;

            var userUrl = from u in db.TB_Url
                where ((u.engagepersonal_ID == id))
                select u;
            foreach (var a in userUrl)
            {
                url = a.url;

            }
            return url;
        }
        public int[] GetCode(string id)
        {
            int[] findCode = new int[2];
            findCode[0] = 2;

            List<TB_Email> mlistData = db.TB_Email.Where((x) => x.Encript_ID == id).ToList();
            for (int i = 0; i < mlistData.Count; i++)
            {
                var index = new TB_Email()
                {
                    StatusAccept = mlistData[0].StatusAccept,
                    engagepersonal_ID = mlistData[0].engagepersonal_ID
                };
                findCode[0] = index.StatusAccept;
                findCode[1] = index.engagepersonal_ID;
            }

            return findCode;
        }
        public ActionResult AddInformationEmp(int page, string sid)
        {
            if (page != 99)
            {
                return HttpNotFound();
            }
            GetDepartment();
            GetChoice_ID_Position();
            GetChoice_TypeEmployment();
            GetChoice_Age();
            GetChoice_ServiceYears();
            GetChoice_Age();
            GetChoice_Career();
            GetChoice_Education();
            GetChoice_Gender();
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            int[] code = GetCode(sid);
            int x = 2;
            x = code[0];
            int chkSection = 0;
            var engageid = code[1];
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInformationEmp", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            chkSection = chkInputData(engageid,1);
            switch (x)
            {
                case 0://ยังไม่ได้ทำแบบทดสอบ 
                    break;

                case 1://ทำแบบทดสอบเรียบร้อยแล้ว

                    break;

                default://2 คือเลข Code ไม่ถูกต้อง
                    return HttpNotFound();

            }
            var viewModel = new DepartmentViewModels()
            {

                Encript_ID = sid,
                StatusAccept = x,
                engagepersonal_ID = engageid,
                Section = chkSection

            };
            return View(viewModel);
        }
        public int chkInputData(int engagepersonal_ID, int sec)
        {
            int rec = 0;
            var posts = from p in db.TB_Answer
                where ((p.engagepersonal_ID == engagepersonal_ID && p.Section == sec))
                select p;
            foreach (var a in posts)
            {
                rec = a.Section;

            }
            return rec;
        }
        public int chkSectionNine(int engagepersonal_ID, int sec)
        {
            int rec = 0;
            //  List<TB_Answer> answerDetail = db.TB_Answer.Where(x => x.engagepersonal_ID == engagepersonal_ID).ToList();
            var posts = from p in db.TB_Answer_Text
                where ((p.engagepersonal_ID == engagepersonal_ID && p.Section == sec))
                select p;
            foreach (var a in posts)
            {
                rec = a.Section;

            }
            return rec;
        }
        [HttpPost]
        public ActionResult Add_Information(string[] values)
        {
            EngagementContext db = new EngagementContext();
            try
            {
                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(values[11]) == 1)
                    {


                            var userDt = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");       // return 08/05/2016 12:56 PM  
                        DateTime oDate = DateTime.ParseExact(userDt, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        for (int i = 0; i < 8; i++)
                        { 
                            
                          var UID = int.Parse((values[0]));
                          var SecID = 1;
                            var ds = (from c in db.TB_Answer where c.engagepersonal_ID == UID && c.Section == SecID && c.Question_ID == i + 1 select c).ToList();
                            if (ds.Count > 0)
                            {
                                foreach (var item in ds)
                                {
                                    item.AnswerResult = Convert.ToInt32(values[i + 1]);
                                    item.dtStamp = oDate;
                                }
                            }
                           db.SaveChanges();

                        }



                    }
                    else
                    {
                        TB_Answer ans = new TB_Answer();
                        int latestRecord = 0;
                        latestRecord = GetLastRecord(1);
                        var userDt = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");       // return 08/05/2016 12:56 PM  
                        DateTime oDate = DateTime.ParseExact(userDt, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                        for (int i = 0; i < 8; i++)
                        {

                            if (latestRecord == 0)
                            {
                                latestRecord = 1;
                            }
                            else
                            {
                                latestRecord = latestRecord + 1;
                            }
                            var engagementData = db.TB_Answer.Where(x => x.Question_ID == i + 1).FirstOrDefault();
                            ans.Question_ID = i + 1;//i
                            ans.engagepersonal_ID = Convert.ToInt32(values[0]);
                            ans.AnswerEngage_ID = latestRecord;//ตัวเลขที่มันรันไปเรื่อยๆๆด้วยมือเรา
                            ans.AnswerResult = Convert.ToInt32(values[i + 1]);
                            ans.Section = 1;
                            ans.dtStamp = oDate;
                            db.TB_Answer.Add(ans);
                            db.SaveChanges();
                        }
                    }



                }
                return Json(new
                {
                    msg = String.Format("Data: {0}", "Save Complete")
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult SaveAllData(string[] values)
        {
            EngagementContext db = new EngagementContext();
            try
            {
                if (ModelState.IsValid)
                {
                    var pageCase = Convert.ToInt32(values[9]);
                    if (Convert.ToInt32(values[9]) != 0)//2 คือ section ที่ 2 ตัวเลขนี้ได้มาจากดึงข้อมูลมาจาก DB ว่ามีแล้วหรือไม่
                    {
                        var userDt = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");       // return 08/05/2016 12:56 PM  
                        DateTime oDate = DateTime.ParseExact(userDt, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        int questionId = Convert.ToInt32(values[7]);
                        for (int i = 0; i < 6; i++, questionId++)
                        {
                            var UID = int.Parse((values[0]));
                            var SecID = int.Parse((values[8]));

                            var ds = (from c in db.TB_Answer where c.engagepersonal_ID == UID && c.Section == SecID && c.Question_ID == questionId select c).ToList();
                            if (ds.Count > 0)
                            {
                                foreach (var item in ds)
                                {
                                    item.AnswerResult = Convert.ToInt32(values[i + 1]);
                                    item.dtStamp = oDate;
                                }
                            }
                            db.SaveChanges();


                        }
                    }
                    else // ถ้าไม่มีข้อมูลให้ insert ใหม่เข้าไป
                    {

                        TB_Answer ans = new TB_Answer();
                        int latestRecord = 0;
                        int questionId = Convert.ToInt32(values[7]);
                        int SaveSection = Convert.ToInt32(values[8]);
                        latestRecord = GetLastRecord(1);
                        //chkeck การเคยเข้าทำแบบทดสอบหรือตอบหน้านี้ไปแล้ว
                        //  chkSection = chkInputData(engageid, 3);
                        for (int i = 0; i < 6; i++, questionId++)//วนใส่ทีละ 6 ข้อ
                        {

                            if (latestRecord == 0)
                            {
                                latestRecord = 1;
                            }
                            else
                            {
                                latestRecord = latestRecord + 1;
                            }
                            var userDt = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");       // return 08/05/2016 12:56 PM  
                            DateTime oDate = DateTime.ParseExact(userDt, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                            ans.Question_ID = questionId;//คำถามข้อที่ 
                            ans.engagepersonal_ID = Convert.ToInt32(values[0]);
                            ans.AnswerEngage_ID = latestRecord;//ตัวเลขที่มันรันไปเรื่อยๆๆด้วยมือเรา
                            ans.AnswerResult = Convert.ToInt32(values[i + 1]);
                            ans.Section = SaveSection;
                            ans.dtStamp = oDate;
                            db.TB_Answer.Add(ans);
                            db.SaveChanges();
                        }
                    }


                    return Redirect(Request.UrlReferrer.ToString());

                }


                return Json(new
                {
                    msg = String.Format("Data: {0}", "Save Complete")
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult updateEngagementStatus(TB_Email objemail)
        {
            Int32 engagementid = (int)TempData["eggageid"];
            var engagementData = db.TB_Email.Where(x => x.engagepersonal_ID == engagementid).FirstOrDefault();
            if (engagementData != null)
            {
                engagementData.StatusAccept = 1;
                db.Entry(engagementData).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public int GetLastRecord(int tabletype)
        {
            int record = 0;
            int max = 0;
            switch (tabletype)
            {
                case 1:
                    List<TB_Answer> ans = ((from c in db.TB_Answer orderby c.AnswerEngage_ID descending select c)).ToList<TB_Answer>();

                    foreach (TB_Answer a in ans)
                    {
                        if (a.AnswerEngage_ID > max)
                        {
                            max = a.AnswerEngage_ID;
                        }

                        record = max;

                    }
                    break;

                case 2:
                    List<TB_Answer_Text> ansText = ((from c in db.TB_Answer_Text orderby c.AnswerEngage_ID descending select c)).ToList<TB_Answer_Text>();
                    foreach (TB_Answer_Text a in ansText)
                    {
                        if (a.AnswerEngage_ID > max)
                        {
                            max = a.AnswerEngage_ID;
                        }

                        record = max;

                    }
                    break;
            }



            return record;
        }
        [HttpPost]
        public ActionResult SaveAllDataText(string[] values)
        {
            EngagementContext db = new EngagementContext();
            try
            {
                TB_Answer ans = new TB_Answer();
                TB_Answer_Text ansText = new TB_Answer_Text();
                int latestRecord = 0;
                int latestRecordText = 0;
                int questionId = Convert.ToInt32(values[5]);
                int SaveSection = Convert.ToInt32(values[6]);
                latestRecord = GetLastRecord(1);
                latestRecordText = GetLastRecord(2);
                var userDt = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");       // return 08/05/2016 12:56 PM  
                DateTime oDate = DateTime.ParseExact(userDt, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                if (Convert.ToInt32(values[7]) != 0)
                {
                    var UID = int.Parse((values[0]));
                    var SecID = int.Parse((values[6]));
                    for (int i = 0; i < 3; i++, questionId++)
                    {


                        var ds = (from c in db.TB_Answer where c.engagepersonal_ID == UID && c.Section == SecID && c.Question_ID == questionId select c).ToList();
                        if (ds.Count > 0)
                        {
                            foreach (var item in ds)
                            {
                                item.AnswerResult = Convert.ToInt32(values[i + 2]);
                                item.dtStamp = oDate;
                            }
                        }
                        db.SaveChanges();


                    }
                    //ไปอัพเดทข้อมูลในตาราง text comment
                    var qtest = int.Parse((values[5]));

                    var dtext = (from c in db.TB_Answer_Text where c.engagepersonal_ID == UID && c.Section == SecID && c.Question_ID == qtest select c).ToList();
                    if (dtext.Count > 0)
                    {
                        foreach (var item in dtext)
                        {
                            item.AnswerResult = (values[1]);
                            item.Section = SaveSection;
                            item.dtStamp = oDate;
                        }
                    }
                    db.SaveChanges();

                    if (questionId == 51)
                    {
                        Int32 engagementid = Convert.ToInt32(values[0]);
                        var engagementData = db.TB_Email.Where(x => x.engagepersonal_ID == engagementid).FirstOrDefault();
                        if (engagementData != null)
                        {
                            engagementData.StatusAccept = 1;
                            engagementData.Survey_Commit = oDate;
                            db.Entry(engagementData).State = EntityState.Modified;
                            db.SaveChanges();

                        }
                    }


                }
                else
                {
                    for (int i = 0; i < 3; i++, questionId++)//วนใส่ทีละ 3 ข้อ
                    {

                        if (latestRecord == 0)
                        {
                            latestRecord = 1;
                        }
                        else
                        {
                            latestRecord = latestRecord + 1;
                        }


                        ans.Question_ID = questionId;//คำถามข้อที่ 
                        ans.engagepersonal_ID = Convert.ToInt32(values[0]);
                        ans.AnswerEngage_ID = latestRecord;//ตัวเลขที่มันรันไปเรื่อยๆๆด้วยมือเรา
                        ans.AnswerResult = Convert.ToInt32(values[i + 2]);
                        ans.Section = SaveSection;
                        ans.dtStamp = oDate;
                        db.TB_Answer.Add(ans);
                        db.SaveChanges();

                    }
                    var qtest = int.Parse((values[5]));
                    DateTime oDateT = DateTime.ParseExact(userDt, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    ansText.Question_ID = qtest;//คำถามข้อที่ 
                    ansText.engagepersonal_ID = Convert.ToInt32(values[0]);
                    ansText.AnswerEngage_ID = latestRecordText;//ตัวเลขที่มันรันไปเรื่อยๆๆด้วยมือเรา
                    ansText.AnswerResult = values[1];
                    ansText.Section = SaveSection;
                    ansText.dtStamp = oDateT;
                    db.TB_Answer_Text.Add(ansText);
                    db.SaveChanges();
                    if (questionId == 51)
                    {
                        Int32 engagementid = Convert.ToInt32(values[0]);
                        var engagementData = db.TB_Email.Where(x => x.engagepersonal_ID == engagementid).FirstOrDefault();
                        if (engagementData != null)
                        {
                            engagementData.StatusAccept = 1;
                            engagementData.Survey_Commit = oDateT;
                            db.Entry(engagementData).State = EntityState.Modified;
                            db.SaveChanges();

                        }
                    }
                }



                return Redirect(Request.UrlReferrer.ToString());






            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetDepartment()
        {
            List<TB_InstitutionUnit_Department> departmentList = db.TB_InstitutionUnit_Department.ToList();
            ViewBag.departmentList = new SelectList(departmentList, "InstitutionUnit_Department_ID", "InstitutionUnit_Department_EN");
        }
        public JsonResult GetDivision(int id)
        {
            var ddlDivision = db.TB_InstitutionUnit_Division.Where(x => x.InstitutionUnit_Department_ID == id).ToList();
            List<SelectListItem> listDivision = new List<SelectListItem>();

            listDivision.Add(new SelectListItem { Text = "--Select Department--", Value = "0" });
            if (ddlDivision != null)
            {
                foreach (var x in ddlDivision)
                {
                    listDivision.Add(new SelectListItem { Text = x.InstitutionUnit_Division_EN, Value = x.InstitutionUnit_Division_ID.ToString() });
                }
            }
            return Json(new SelectList(listDivision, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        public void GetChoice_ID_Position()
        {
            List<TB_Choice> choiceList = db.TB_Choice.Where(x => x.Question_ID == 2).ToList();
            ViewBag.ChoiceList = new SelectList(choiceList, "Choice_ID", "ChoiceTextEN");
        }
        public void GetChoice_TypeEmployment()
        {
            List<TB_Choice> choiceListPosition = db.TB_Choice.Where(x => x.Question_ID == 3).ToList();
            ViewBag.ChoiceListPosition = new SelectList(choiceListPosition, "Choice_ID", "ChoiceTextEN");
        }
        public void GetChoice_Age()
        {
            List<TB_Choice> choiceListAge = db.TB_Choice.Where(x => x.Question_ID == 4).ToList();
            ViewBag.ChoiceListAge = new SelectList(choiceListAge, "Choice_ID", "ChoiceTextEN");
        }
        public void GetChoice_ServiceYears()
        {
            List<TB_Choice> choiceListServiceYears = db.TB_Choice.Where(x => x.Question_ID == 5).ToList();
            ViewBag.ChoiceListServiceYears = new SelectList(choiceListServiceYears, "Choice_ID", "ChoiceTextEN");
        }
        public void GetChoice_Gender()
        {
            List<TB_Choice> choiceListGender = db.TB_Choice.Where(x => x.Question_ID == 6).ToList();
            ViewBag.choiceListGender = new SelectList(choiceListGender, "Choice_ID", "ChoiceTextEN");
        }
        public void GetChoice_Education()
        {
            List<TB_Choice> choiceListEducation = db.TB_Choice.Where(x => x.Question_ID == 7).ToList();
            ViewBag.choiceListEducation = new SelectList(choiceListEducation, "Choice_ID", "ChoiceTextEN");
        }
        public void GetChoice_Career()
        {
            List<TB_Choice> choiceListCareer = db.TB_Choice.Where(x => x.Question_ID == 8).ToList();
            ViewBag.choiceListCareer = new SelectList(choiceListCareer, "Choice_ID", "ChoiceTextEN");
        }
        public ActionResult AddInEngageFirst(int page, string sid)
        {
            //p คือ page หน้าที่ผู้ใช้ทำงานอยู่
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            if (page != 60)
            {
                return HttpNotFound();
            }
            int[] code = GetCode(sid);
            int codeinvalid = 2;
            int endofTest = 0;
            endofTest = code[0];
            codeinvalid = code[0];
            int chkSection = 0;
            var engageid = code[1];
            if (page == null)
            {
                return HttpNotFound();
            }
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInEngageFirst", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            else
            {
                updateUrl(fullUrl, engageid);
            }
            if (codeinvalid == 2)
                return HttpNotFound();
            chkSection = chkInputData(engageid,2);
            List<TB_Question> questionName = db.TB_Question.Where(x => x.Section == 2).ToList();
            List<TB_Choice> choiceName = db.TB_Choice.Where(c => c.Question_ID == 9).ToList();
            
            var viewModel = new questionchoiceViewModel();
            viewModel.questionDetail = questionName;
            viewModel.choiceDetail = choiceName;
            viewModel.Section = chkSection;
            viewModel.StatusAccept = endofTest;
            viewModel.engagepersonal_ID = engageid;
            viewModel.Encript_ID = sid;

            return View(viewModel);
        }
        public ActionResult AddInEngageSecond(int page, string sid)
        {
            //p คือ page หน้าที่ผู้ใช้ทำงานอยู่
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            if (page != 58)
            {
                return HttpNotFound();
            }
            int[] code = GetCode(sid);
            int codeinvalid = 2;
            int endofTest = 0;
            endofTest = code[0];
            codeinvalid = code[0];
            int chkSection = 0;
            var engageid = code[1];
            if (page == null)
            {
                return HttpNotFound();
            }
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInEngageSecond", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            else
            {
                //update url
                updateUrl(fullUrl, engageid);
            }
            if (codeinvalid == 2)
                return HttpNotFound();
            chkSection = chkInputData(engageid,3);
            List<TB_Question> questionName = db.TB_Question.Where(x => x.Section == 2).ToList();
            List<TB_Choice> choiceName = db.TB_Choice.Where(c => c.Question_ID == 9).ToList();
          
            
            var viewModel = new questionchoiceViewModel();
            viewModel.questionDetail = questionName;
            viewModel.choiceDetail = choiceName;
            viewModel.Section = chkSection;
            viewModel.StatusAccept = endofTest;
            viewModel.engagepersonal_ID = engageid;
            viewModel.Encript_ID = sid;

            return View(viewModel);
        }

        public ActionResult AddInEngageThird(int page, string sid)

        {
            //p คือ page หน้าที่ผู้ใช้ทำงานอยู่
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            if (page != 60)
            {
                return HttpNotFound();
            }
            int[] code = GetCode(sid);
            int codeinvalid = 2;
            int endofTest = 0;
            endofTest = code[0];
            codeinvalid = code[0];
            int chkSection = 0;
            var engageid = code[1];
            if (page == null)
            {
                return HttpNotFound();
            }
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInEngageThird", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            else
            {
                //update url
                updateUrl(fullUrl, engageid);
            }
            if (codeinvalid == 2)
                return HttpNotFound();
            chkSection = chkInputData(engageid,4);
            List<TB_Question> questionName = db.TB_Question.Where(x => x.Section == 2).ToList();
            List<TB_Choice> choiceName = db.TB_Choice.Where(c => c.Question_ID == 9).ToList();
            
            var viewModel = new questionchoiceViewModel();
            viewModel.questionDetail = questionName;
            viewModel.choiceDetail = choiceName;
            viewModel.Section = chkSection;
            viewModel.StatusAccept = endofTest;
            viewModel.engagepersonal_ID = engageid;
            viewModel.Encript_ID = sid;

            return View(viewModel);
        }

        public ActionResult AddInEngageFour(int page, string sid)

        {
            //p คือ page หน้าที่ผู้ใช้ทำงานอยู่
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            if (page != 1001)
            {
                return HttpNotFound();
            }
            int[] code = GetCode(sid);
            int codeinvalid = 2;
            int endofTest = 0;
            int chkSection = 0;
            int sectionParameter = 5;
           
            endofTest = code[0];
            codeinvalid = code[0];

            var engageid = code[1];
            if (page == null)
            {
                return HttpNotFound();
            }
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInEngageFour", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            else
            {
                //update url
                updateUrl(fullUrl, engageid);
            }
            if (codeinvalid == 2)
                return HttpNotFound();
            chkSection = chkInputData(engageid,5);
            List<TB_Question> questionName = db.TB_Question.Where(x => x.Section == 2).ToList();
            List<TB_Choice> choiceName = db.TB_Choice.Where(c => c.Question_ID == 9).ToList();
            
            var viewModel = new questionchoiceViewModel();
            viewModel.questionDetail = questionName;
            viewModel.choiceDetail = choiceName;
            viewModel.Section = chkSection;
            viewModel.passSection = sectionParameter;
            viewModel.StatusAccept = endofTest;
            viewModel.engagepersonal_ID = engageid;
            viewModel.Encript_ID = sid;

            return View(viewModel);
        }
        public ActionResult AddInEngageFive(int page, string sid)

        {
            //p คือ page หน้าที่ผู้ใช้ทำงานอยู่
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            if (page != 57)
            {
                return HttpNotFound();
            }
            int[] code = GetCode(sid);
            int codeinvalid = 2;
            int endofTest = 0;
            int chkSection = 0;
            int sectionParameter = 6;
           
            endofTest = code[0];
            codeinvalid = code[0];

            var engageid = code[1];
            if (page == null)
            {
                return HttpNotFound();
            }
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInEngageFive", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            else
            {
                //update url
                updateUrl(fullUrl, engageid);
            }
            if (codeinvalid == 2)
                return HttpNotFound();
            chkSection = chkInputData(engageid,6);
            List<TB_Question> questionName = db.TB_Question.Where(x => x.Section == 2).ToList();
            List<TB_Choice> choiceName = db.TB_Choice.Where(c => c.Question_ID == 9).ToList();
            
            var viewModel = new questionchoiceViewModel();
            viewModel.questionDetail = questionName;
            viewModel.choiceDetail = choiceName;
            viewModel.Section = chkSection;
            viewModel.passSection = sectionParameter;
            viewModel.StatusAccept = endofTest;
            viewModel.engagepersonal_ID = engageid;
            viewModel.Encript_ID = sid;

            return View(viewModel);
        }
        public ActionResult AddInEngageSix(int page, string sid)

        {
            //p คือ page หน้าที่ผู้ใช้ทำงานอยู่
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            if (page != 231)
            {
                return HttpNotFound();
            }
            int[] code = GetCode(sid);
            int codeinvalid = 2;
            int endofTest = 0;
            int chkSection = 0;
            int sectionParameter = 7;
           
            endofTest = code[0];
            codeinvalid = code[0];

            var engageid = code[1];
            if (page == null)
            {
                return HttpNotFound();
            }
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInEngageSix", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            else
            {
                //update url
                updateUrl(fullUrl, engageid);
            }
            if (codeinvalid == 2)
                return HttpNotFound();
            chkSection = chkInputData(engageid,7);
            List<TB_Question> questionName = db.TB_Question.Where(x => x.Section == 2).ToList();
            List<TB_Choice> choiceName = db.TB_Choice.Where(c => c.Question_ID == 9).ToList();
           
            var viewModel = new questionchoiceViewModel();
            viewModel.questionDetail = questionName;
            viewModel.choiceDetail = choiceName;
            viewModel.Section = chkSection;
            viewModel.passSection = sectionParameter;
            viewModel.StatusAccept = endofTest;
            viewModel.engagepersonal_ID = engageid;
            viewModel.Encript_ID = sid;

            return View(viewModel);
        }
        public ActionResult AddInEngageSeven(int page, string sid)

        {
            //p คือ page หน้าที่ผู้ใช้ทำงานอยู่
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            if (page != 303)
            {
                return HttpNotFound();
            }
            int[] code = GetCode(sid);
            int codeinvalid = 2;
            int endofTest = 0;
            int chkSection = 0;
            int sectionParameter = 8;
           
            endofTest = code[0];
            codeinvalid = code[0];

            var engageid = code[1];
            if (page == null)
            {
                return HttpNotFound();
            }
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInEngageSeven", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            else
            {
                //update url
                updateUrl(fullUrl, engageid);
            }
            if (codeinvalid == 2)
                return HttpNotFound();
            chkSection = chkInputData(engageid,8);
            List<TB_Question> questionName = db.TB_Question.Where(x => x.Section == 2).ToList();
            List<TB_Choice> choiceName = db.TB_Choice.Where(c => c.Question_ID == 9).ToList();
           
            var viewModel = new questionchoiceViewModel();
            viewModel.questionDetail = questionName;
            viewModel.choiceDetail = choiceName;
            viewModel.Section = chkSection;
            viewModel.passSection = sectionParameter;
            viewModel.StatusAccept = endofTest;
            viewModel.engagepersonal_ID = engageid;
            viewModel.Encript_ID = sid;

            return View(viewModel);
        }
        public ActionResult AddInEngageEight(int page, string sid)

        {

            //p คือ page หน้าที่ผู้ใช้ทำงานอยู่
            GetChoice_topiccomment();
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            if (page != 601)
            {
                return HttpNotFound();
            }
            int[] code = GetCode(sid);
            int codeinvalid = 2;
            int endofTest = 0;
            int chkSection = 0;
            int sectionParameter = 9;

            endofTest = code[0];
            codeinvalid = code[0];

            var engageid = code[1];
            if (page == null)
            {
                return HttpNotFound();
            }
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInEngageEight", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            else
            {
                //update url
                updateUrl(fullUrl, engageid);
            }
            if (codeinvalid == 2)
                return HttpNotFound();
            chkSection = chkInputData(engageid, 9);
            List<TB_Question> questionName = db.TB_Question.Where(x => x.Section == 3).ToList();

            var viewModel = new questionchoiceViewModel();
            viewModel.questionDetail = questionName;
            viewModel.Section = chkSection;
            viewModel.passSection = sectionParameter;
            viewModel.StatusAccept = endofTest;
            viewModel.engagepersonal_ID = engageid;
            viewModel.Encript_ID = sid;

            return View(viewModel);
        }
        public ActionResult AddInEngageNine(int page, string sid)
        {
            //p คือ page หน้าที่ผู้ใช้ทำงานอยู่
            GetChoice_topiccomment();
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            if (page != 501)
            {
                return HttpNotFound();
            }
            int[] code = GetCode(sid);
            int codeinvalid = 2;
            int endofTest = 0;
            int chkSection = 0;
            int sectionParameter = 10;

            endofTest = code[0];
            codeinvalid = code[0];

            var engageid = code[1];
            if (page == null)
            {
                return HttpNotFound();
            }
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInEngageNine", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            else
            {
                //update url
                updateUrl(fullUrl, engageid);
            }
            if (codeinvalid == 2)
                return HttpNotFound();
            chkSection = chkInputData(engageid, 10);
            List<TB_Question> questionName = db.TB_Question.Where(x => x.Section == 3).ToList();

            var viewModel = new questionchoiceViewModel();
            viewModel.questionDetail = questionName;
            viewModel.Section = chkSection;
            viewModel.passSection = sectionParameter;
            viewModel.StatusAccept = endofTest;
            viewModel.engagepersonal_ID = engageid;
            viewModel.Encript_ID = sid;

            return View(viewModel);
        }
        public ActionResult AddInEngageTen(int page, string sid)
        {
            GetChoice_topiccomment();
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            if (page != 506)
            {
                return HttpNotFound();
            }
            int[] code = GetCode(sid);
            int codeinvalid = 2;
            int endofTest = 0;
            int chkSection = 0;
            int sectionParameter = 11;

            endofTest = code[0];
            codeinvalid = code[0];

            var engageid = code[1];
            if (page == null)
            {
                return HttpNotFound();
            }
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("AddInEngageTen", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
            }
            else
            {
                //update url
                updateUrl(fullUrl, engageid);
            }
            if (codeinvalid == 2)
                return HttpNotFound();
            chkSection = chkInputData(engageid, 11);
            List<TB_Question> questionName = db.TB_Question.Where(x => x.Section == 3).ToList();

            var viewModel = new questionchoiceViewModel();
            viewModel.questionDetail = questionName;
            viewModel.Section = chkSection;
            viewModel.passSection = sectionParameter;
            viewModel.StatusAccept = endofTest;
            viewModel.engagepersonal_ID = engageid;
            viewModel.Encript_ID = sid;

            return View(viewModel);
        }

        public ActionResult EndofTest(int page, string sid)
        {
            bool b1 = string.IsNullOrEmpty(sid);
            if (sid == string.Empty || b1 == true)
                return HttpNotFound();
            int[] code = GetCode(sid);
            if (page != 48)
            {
                return HttpNotFound();
            }
            int codeinvalid = 2;
            int endofTest = 0;
            endofTest = code[0];
            var engageid = code[1];
            var currentUrl = string.Empty;
            var fullUrl = this.Url.Action("EndofTest", "AddEngagementEN", null, this.Request.Url.Scheme);
            currentUrl = GetUrl(engageid);
            if (currentUrl == string.Empty)
            {
                SaveCurrentUrl(fullUrl, engageid);
                var userDt = DateTime.Now.ToString("MM/dd/yyyy");       // return 08/05/2016 12:56 PM  
                DateTime oDateT = DateTime.ParseExact(userDt, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                Int32 engagementid = engageid;
                var engagementData = db.TB_Email.Where(x => x.engagepersonal_ID == engagementid).FirstOrDefault();
                if (engagementData != null)
                {
                    engagementData.StatusAccept = 1;
                    engagementData.Survey_Commit = oDateT;
                    db.Entry(engagementData).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            else
            {
                //update url
                updateUrl(fullUrl, engageid);
                var userDt = DateTime.Now.ToString("MM/dd/yyyy");       // return 08/05/2016 12:56 PM  
                DateTime oDateT = DateTime.ParseExact(userDt, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                Int32 engagementid = engageid;
                var engagementData = db.TB_Email.Where(x => x.engagepersonal_ID == engagementid).FirstOrDefault();
                if (engagementData != null)
                {
                    engagementData.StatusAccept = 1;
                    engagementData.Survey_Commit = oDateT;
                    db.Entry(engagementData).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            var viewModel = new ParameterValue();
            viewModel.StatusAccept = endofTest;
            viewModel.Encript_ID = sid;
            return View(viewModel);
        }
        public void GetChoice_topiccomment()
        {
            List<TB_Choice> choiceListComment = db.TB_Choice.Where(x => x.Question_ID == 43).ToList();
            ViewBag.choiceListComment = new SelectList(choiceListComment, "Choice_ID", "ChoiceTextEN");
        }

    }
}