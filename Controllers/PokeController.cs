using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace pokeInfo{

using Newtonsoft.Json;

    public static class SessionExtensions // Somewhere in your namespace, outside other classes
    {
        public static void SetObjectAsJson(this ISession session, string key, object value) // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
        {
            session.SetString(key, JsonConvert.SerializeObject(value)); // This helper function simply serializes theobject to JSON and stores it as a string in session
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)  // generic type T is a stand-in indicating that we need to specify the type on retrieval
        {
            string value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);   // Upon retrieval the object is deserialized based on the type we specified
        }
    }

    public class PokeController : Controller{
        [Route("/")]
        public IActionResult Index() {
            return View("index");
        }

        [HttpPost]
        [Route("/pokemon")]
        public IActionResult ID(int pID){
            int PokeId = pID;
            return Redirect("/pokemon/" + pID);
        }

        [Route("/pokemon/{pID}")]
        public IActionResult Info(int pID){
            var PokeInfo = new JObject();
            WebRequest.GetPokemonDataAsync(pID, ApiResponse =>
                {
                    PokeInfo = ApiResponse;
                }
            ).Wait();

            if(PokeInfo["name"] == null){
                return Redirect("/");
            }
            else{
                if(PokeInfo["types"].Count() > 1){
                    string sType = (string)PokeInfo["types"][1]["type"]["name"];
                    ViewBag.sType = sType.First().ToString().ToUpper() + sType.Substring(1);
                }

                string name = (string)PokeInfo["name"];
                string pType = (string)PokeInfo["types"][0]["type"]["name"];

                ViewBag.Name = name.First().ToString().ToUpper() + name.Substring(1);
                ViewBag.pType = pType.First().ToString().ToUpper() + pType.Substring(1);
                ViewBag.Height = (string)PokeInfo["height"];
                ViewBag.Weight = (string)PokeInfo["weight"];
                ViewBag.PokeInfo = PokeInfo;
                return View("info");
            }
        }

    [HttpGet]
    [Route("/random")]
        public IActionResult Random(int pID)
        {
            Random random = new Random();
            int randNum = random.Next(1,721);
            var pokemon = new Dictionary<string, object>();
            WebRequest.GetPokemonDataAsync(randNum, PokeInfo =>
            {
                var temp = new List<string>();
                string name = (string)PokeInfo["name"];
                string pType = (string)PokeInfo["types"][0]["type"]["name"];
                pokemon["name"] = name.First().ToString().ToUpper() + name.Substring(1);
                pokemon["primary_type"] = pType.First().ToString().ToUpper() + pType.Substring(1);
                if(PokeInfo["types"].Count() > 1)
                {
                    string sType = (string)PokeInfo["types"][1]["type"]["name"];
                    pokemon["secondary_type"] = sType.First().ToString().ToUpper() + sType.Substring(1);
                }
                pokemon["height"] = (string)PokeInfo["height"];
                pokemon["weight"] = (string)PokeInfo["weight"];

                for(int i=0; i<PokeInfo["types"].Count(); i++){
                    string tWord = (string)PokeInfo["types"][i]["type"]["name"];
                    temp.Add(tWord.First().ToString().ToUpper() + tWord.Substring(1));
                }
                pokemon["types"] = temp;
                }
            ).Wait();
            if(pokemon["name"] == null)
            {
                return Redirect("/");
            }
            else
            {
                return Json(pokemon);  
            } 
        }  
    }
}