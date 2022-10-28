using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CVMLabs.Models;
using Microsoft.AspNetCore.Identity;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using CVMLabs.Domain.Entities;
using CVMLabs.Service;
using Microsoft.EntityFrameworkCore;

namespace CVMLabs.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DataManager _dataManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DataManager dataManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dataManager = dataManager;
            _context = context;
        }

        /* ====== (GET Request) Sign in ====== */
        [AllowAnonymous]
        public IActionResult Signin()
        {
            var user = User.Identity;
            if (user is not null && user.IsAuthenticated) return Redirect("/");
            return View(new LoginViewModel());
        }

        /* ====== (POST Request) Sign in ====== */
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signin(LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(model.Username);
                if (user is not null)
                {
                    await _signInManager.SignOutAsync();
                    SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded) return Redirect(returnUrl ?? "/");
                }
                ModelState.AddModelError(nameof(LoginViewModel.Password), "Incorrect username or password");
            }
            return View(model);
        }

        /* ====== (GET Request) Sign out ====== */
        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        /* ====== (GET Request) Sign up ====== */
        [AllowAnonymous]
        public IActionResult Signup() => View();

        /* ====== (POST Request) Sign up ====== */
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signup(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new() { UserName = model.Username, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return Redirect("/");
                }
                foreach (var error in result.Errors) ModelState.AddModelError(nameof(RegisterViewModel), error.Description);
            }
            return View(model);
        }

        /* ====== (GET Request) Profile ====== */
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            UserInfo userInfo = model.UserInfoModel;

            userInfo.Username = user.UserName;
            userInfo.Email = user.Email;

            ModelState.Clear();
            return View(model);
        }

        /* ====== (POST Request) Profile Personal Info Changing ====== */
        [HttpPost]
        public async Task<IActionResult> ProfilePersonalInfoChanging(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            UserInfo userInfo = model.UserInfoModel;

            if (user.UserName != userInfo.Username) await _userManager.SetUserNameAsync(user, userInfo.Username);
            if (user.Email != userInfo.Email)
            {
                var emailToken = await _userManager.GenerateChangeEmailTokenAsync(user, userInfo.Email);
                await _userManager.ChangeEmailAsync(user, userInfo.Email, emailToken);
            }
            return RedirectToAction("Profile");
        }

        /* ====== (POST Request) Profile Password Changing ====== */
        [HttpPost, ActionName("Profile")]
        public async Task<IActionResult> ProfilePasswordChanging(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            ModelState.Remove("UserInfoModel.Username");
            ModelState.Remove("UserInfoModel.Email");

            if (ModelState.IsValid)
            {
                UserPassword userPassword = model.UserPasswordModel;

                var result = await _userManager.ChangePasswordAsync(user, userPassword.OldPassword, userPassword.NewPassword);
                if (result.Succeeded) return RedirectToAction("Profile");

                foreach (var error in result.Errors) ModelState.AddModelError("UserPasswordModel.ConfirmNewPassword", error.Description);
            }
            return View(model);  
        }

        /* ====== (POST Request) Create New Subject ====== */
        [HttpPost]
        public async Task<IActionResult> CreateNewSubject(SubjectModel model)
        {
            var user = await _userManager.GetUserAsync(User); // TODO: Add model validation
            SubjectModel subject = model;
            user.UserSubjects?.Add(subject);
            
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return StatusCode(500);
            return RedirectToAction("Profile");
        }

        /* ====== (POST Request) Delete Subject ====== */
        [HttpPost]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Include(s => s.UserSubjects).SingleOrDefaultAsync(u => u.Id == userId);
            var subjectToDelete = user?.UserSubjects.Find(e => e.Id == id);

            if (subjectToDelete is null) return Content("Access denied");

            _dataManager.Subjects.DeleteSubjectById(subjectToDelete.Id);
            return RedirectToAction("Profile");
        }

        /* ====== (GET Request) Edit Subject Pop-up Call ====== */
        public IActionResult EditSubjectModalCall(int id) // TODO: Add check if requested id is related to user
        {
            SubjectModel model = _dataManager.Subjects.GetSubjectById(id);
            if (model is null) return Content("Subject not found");
            return PartialView("Partial/EditSubjectModalPartial", model);
        }

        /* ====== (POST Request) Edit Subject ====== */
        [HttpPost]
        public async Task<IActionResult> EditSubject(SubjectModel model, int id)
        {
            model.Id = id;
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Include(s => s.UserSubjects).SingleOrDefaultAsync(u => u.Id == userId);
            var subjectToEdit = user?.UserSubjects.Find(e => e.Id == model.Id);

            if (subjectToEdit is null) return Content("Access denied");

            subjectToEdit.SubjectTitle = model.SubjectTitle;
            _dataManager.Subjects.SaveSubject(subjectToEdit);
            return RedirectToAction("Profile");
        }

        /* ====== (POST Request) Create New Student ====== */
        [HttpPost]
        public async Task<IActionResult> CreateNewStudent(StudentModel model)
        {
            if (!ModelState.IsValid) return View("Profile", new ProfileViewModel() { StudentModel = model });

            var user = await _userManager.GetUserAsync(User);
            StudentModel student = model;
            user.UserStudents?.Add(student);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return StatusCode(500);
            return RedirectToAction("Profile");
        }

        /* ====== (POST Request) Delete Student ====== */
        [HttpPost]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Include(s => s.UserStudents).SingleOrDefaultAsync(u => u.Id == userId);
            var studentToDelete = user?.UserStudents.Find(e => e.Id == id);

            if (studentToDelete is null) return Content("Access denied"); 

            _dataManager.Students.DeleteStudentById(studentToDelete.Id);
            return RedirectToAction("Profile");
        }

        /* ====== (GET Request) Edit Student Pop-up Call ====== */
        public IActionResult EditStudentModalCall(int id) // TODO: Add check if requested id is related to user
        {
            StudentModel model = _dataManager.Students.GetStudentById(id);
            if (model is not null) return PartialView("Partial/EditStudentModalPartial", model);
            return Content("Student not found");
        }

        /* ====== (POST Request) Edit Student ====== */
        [HttpPost]
        public async Task<IActionResult> EditStudent(StudentModel model, int id)
        {
            model.Id = id;
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users.Include(s => s.UserStudents).SingleOrDefaultAsync(u => u.Id == userId);
            var studentToEdit = user?.UserStudents.Find(e => e.Id == model.Id);

            if (studentToEdit is null) return Content("Access denied");

            studentToEdit.Name = model.Name;
            _dataManager.Students.SaveStudent(studentToEdit);
            return RedirectToAction("Profile");
        }
    }
}