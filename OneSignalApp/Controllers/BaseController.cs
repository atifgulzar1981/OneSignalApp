﻿using Microsoft.AspNetCore.Mvc;

namespace OneSignalApp.Controllers
{
  public abstract class BaseController : Controller
  {
    protected void FlashSuccess(string message)
    {
      TempData[AppConstants.SUCCESS_MSG_KEY] = message;
    }

    protected void FlashError(string message)
    {
      TempData[AppConstants.ERROR_MSG_KEY] = message;
    }
  }
}