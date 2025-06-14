﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using ProjectThread.Migrations;
using ProjectThread.Models;
using System.Security.Claims;
using System.Threading;

namespace ProjectThread.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Editor(Guid? id)
        {
            var user = _context.Users.Where(a => a.UserGUID == id).FirstOrDefault();
            if (user == null)
            {
                return View();
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Editor(User user)
        {
            if (user.UserID == 0)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            else
            {
                _context.Users.Where(a => a.UserGUID == user.UserGUID)
                                    .ExecuteUpdate(setters => setters
                                        .SetProperty(a => a.Name, user.Name)
                                        .SetProperty(a => a.Email, user.Email)
                                        .SetProperty(a => a.Phone, user.Phone)
                                        .SetProperty(a => a.Gender, user.Gender)
                                        .SetProperty(a => a.Address1, user.Address1)
                                        .SetProperty(a => a.City, user.City)
                                        .SetProperty(a => a.State, user.State)
                                        .SetProperty(a => a.Country, user.Country)
                                        .SetProperty(a => a.Dob, user.Dob)
                                        .SetProperty(a => a.Password, user.Password));
            }
            return RedirectToAction("Profile", "Users");
        }
        [HttpPost]
        public IActionResult Delete(Guid? id)
        {
            int userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value);
            if (userIdClaim > 0)
            {
                var user = _context.Users.Where(a => a.UserGUID == id).FirstOrDefault();
                if (user == null)
                {
                    return Json(new { success = true, message = "User Not Found" });
                }
                _context.Users.Remove(user);
                _context.SaveChanges();
                return Json(new { success = true, message = "User Deleted" });
            }
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult Profile()
        {
            //var ClaimName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            //int ClaimId = int.Parse(User.Claims.FirstOrDefault(static c => c.Type == "UserID").Value);
            var userGuidClaim = User.Claims.FirstOrDefault(c => c.Type == "UserGUID")?.Value;
            if (!Guid.TryParse(userGuidClaim, out Guid userGuid))
            {
                return Unauthorized();
            }
            var user = _context.Users.FirstOrDefault(a => a.UserGUID == userGuid);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View(user);
        }


        public IActionResult Notes(Guid? id)
        {

            int userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value);
            if (userIdClaim > 0)
            {
                if (id != null)
                {
                    var note = _context.Notes.Where(a => a.NoteGUID == id).FirstOrDefault();
                    return View(note);
                }
                var Notes = _context.Notes.Where(a => a.UserID == userIdClaim).ToList();
                ViewBag.Notes = Notes;
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }


        public IActionResult NoteEditor(Models.Note note)
        {
            if (note.NoteId == 0)
            {
                int userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value);
                note.UserID = userIdClaim;
                _context.Notes.Add(note);
                _context.SaveChanges();

            }
            else
            {
                _context.Notes.Where(a => a.NoteGUID == note.NoteGUID)
                    .ExecuteUpdate(s => s.SetProperty(a => a.Heading, note.Heading)
                                        .SetProperty(a => a.Body, note.Body));
            }
            return RedirectToAction("Notes", "Users");

        }

        public IActionResult Friends()
        {
            int userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value);
            if (userIdClaim > 0)
            {
                var Friends = _context.Friends.Where(a => a.UserID == userIdClaim).ToList();
                ViewData["Friends"] = Friends;

                var friendIds = Friends.Select(f => f.FriendUserID).Distinct().ToList();
                var OtherUsers = _context.Users.Where(u => u.UserID != userIdClaim && !friendIds.Contains(u.UserID)).ToList();
                ViewData["OtherUsers"] = OtherUsers;

                var friendRequests = _context.FriendRequests.Where(f => f.FriendUserID == userIdClaim && f.IsConfirm==0).
                    GroupBy(r => r.FriendUserID).Select(g => g.First()).ToList();
                if (friendRequests.Any())
                {
                    ViewData["friendReqList"] = friendRequests;
                }
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        public IActionResult OpenChat(int id)
        {
            int userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value);
            HttpContext.Session.SetInt32("FriendUserID", id);
            if (id > 0)
            {
                var threads = _context.Threads
                     .Where(a =>
                         (a.SenderID == userIdClaim && a.ReceiverID == id) ||
                         (a.SenderID == id && a.ReceiverID == userIdClaim))
                     .ToList();
                if (threads != null)
                {
                    ViewData["threads"] = threads;
                    ViewData["ReceiverName"] = _context.Users.Where(a => a.UserID == id).Select(b => b.Name).FirstOrDefault();
                    ViewData["senderID"] = userIdClaim;
                    ViewData["FriendUserID"] = id;
                    return View(threads);
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "No threads found !!"
                    });
                }

            }
            return RedirectToAction("Friends", "Users");
        }

        public IActionResult AddThread(IFormCollection collection)
        {
            int userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value);
            int? friendUserId = HttpContext.Session.GetInt32("FriendUserID");
            var friendName = _context.Users.Where(a => a.UserID == friendUserId).Select(b => b.Name).FirstOrDefault();
            Models.Thread thread = new Models.Thread();
            thread.ReceiverID = friendUserId.Value;
            thread.ReceiverName = friendName;
            thread.SenderID = userIdClaim;
            thread.ThreadGUID = Guid.NewGuid();
            thread.Direction = Convert.ToInt32(collection["Direction"]);
            thread.ThreadBody = collection["Body"];
            _context.Threads.Add(thread);
            _context.SaveChanges();
            ViewData["CurrentFriend"] = friendUserId;
            return RedirectToAction("OpenChat", "Users", new { id = friendUserId.Value });
        }





        public IActionResult AddToFriend(int id)
        {
            int userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value);
            string userNameClaim = User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
            var checkFriend = _context.Friends.Where(a => a.UserID == userIdClaim && a.FriendUserID == id).FirstOrDefault();
            if (checkFriend == null)
            {
                if (id > 0 && userIdClaim > 0)
                {
                    string friendName = _context.Users.Where(a => a.UserID == id).Select(b => b.Name).FirstOrDefault();
                    Friend friend1 = new Friend();
                    friend1.FriendUserID = id;
                    friend1.UserID = userIdClaim;
                    friend1.UserName = userNameClaim;
                    friend1.FriendName = friendName;
                    _context.Friends.Add(friend1);
                    _context.SaveChanges();

                    Friend friend2 = new Friend();
                    friend2.FriendUserID = userIdClaim;
                    friend2.UserID =id ;
                    friend2.UserName = friendName ;
                    friend2.FriendName = userNameClaim;
                    _context.Friends.Add(friend2);
                    _context.SaveChanges();

                    var request = _context.FriendRequests.Where(a => a.UserID == id && a.FriendUserID == userIdClaim).FirstOrDefault();
                    if (request != null)
                    {
                     _context.FriendRequests.Where(a => a.UserID == id && a.FriendUserID == userIdClaim)
                                .ExecuteUpdate(setters => setters.SetProperty(x => x.IsConfirm, 1)); 
                     _context.FriendRequests.Where(a => a.UserID == userIdClaim && a.FriendUserID ==id )
                                .ExecuteUpdate(setters => setters.SetProperty(x => x.IsConfirm, 1));
                    }
                    string msgTrue = "You are now Friend with " + friend1.FriendName;
                    return Json(new { success = true, message = msgTrue });
                }
                return RedirectToAction("Login", "Auth");
            }
            string msgFalse = "You are allready friend with user " + checkFriend.FriendName;
            return Json(new { success = true, message = msgFalse });
        }
        public IActionResult SendRequest(int id)
        {
            int userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value);
            string userNameClaim = User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
            var checkReq = _context.FriendRequests.Where(a => a.UserID == userIdClaim && a.FriendUserID == id).FirstOrDefault();
            if (checkReq == null)
            {
                if (id > 0 && userIdClaim > 0)
                {
                    var friendName = _context.Users.Where(a => a.UserID == id).Select(b => b.Name).FirstOrDefault();
                    FriendRequest request = new FriendRequest();
                    request.FriendUserID = id;
                    request.FriendName = friendName;
                    request.UserName = userNameClaim;
                    request.UserID = userIdClaim;
                    _context.FriendRequests.Add(request);
                    _context.SaveChanges();
                }
                return Json(new { success = true, message = "Request Sent" });
            }
            return Json(new { success = false, message = " You have allready sent Request to this user." });
        }

        public IActionResult DeleteFriend(int id)
        {
            int userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value);
            if (id > 0 && userIdClaim > 0)
            {
                
                var friends = _context.Friends.Where(a => a.UserID == userIdClaim && a.FriendUserID == id ||
                a.UserID==id && a.FriendUserID==userIdClaim).ToList();
                if (friends != null)
                {
                    _context.Friends.RemoveRange(friends);
                    _context.SaveChanges();
                    var requests = _context.FriendRequests.Where(a => a.UserID == userIdClaim || a.UserID == id
                                          || a.FriendUserID == userIdClaim || a.FriendUserID == id).ToList();
                    if (requests != null)
                    {
                        _context.FriendRequests.RemoveRange(requests);
                        _context.SaveChanges();

                    }
                    return Json(new { success = true, message = "Usre Deleted !!" });
                }
                return Json(new { success = false, message = "User not found !!" });
            }
            return RedirectToAction("Login", "Login");

        }

        public IActionResult DeleteNotes(Guid? id)
        {
            int userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value);
            if (userIdClaim > 0)
            {
                var note = _context.Notes.Where(a => a.UserID == userIdClaim && a.NoteGUID == id).FirstOrDefault();
                if (note != null)
                {
                    _context.Notes.Remove(note);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Notes Deleted !!" });
                }
                return Json(new { success = false, message = "Note not found !!" });
            }
            return RedirectToAction("Login", "Login");

        }








    }
}
