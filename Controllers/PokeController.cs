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
            // var test = new Dictionary<string, object>();
            // WebRequestEx.GetPokemonDataAsyncEx(2, PokeInfo =>
            // {
            //     test = PokeInfo;
            //     Console.WriteLine(test);
            // }
            //     ).Wait();
            return View("index");
        }

        [HttpPost]
        [Route("/pokemon")]
        public IActionResult ID(int pID)
        {
            if((pID > 0) && (pID < 722))
            {
                return Redirect("/pokemon/" + pID);
            }
            else
            {
                return Redirect("/");
            }
        }

        [HttpGet]
        [Route("/pokemon")]
        public IActionResult Inv()
        {
            return Redirect("/");
        }
        
        [Route("/pokemon/{pID}")]
        public IActionResult Info(int pID){
            var pokemon = new Dictionary<string, object>();
            if((pID < 1) || (pID > 721))
            {
                return Redirect("/");
            }
            WebRequest.GetPokemonDataAsync(pID, PokeInfo =>
            {
                var tempType = new List<string>();
                var tempSprite = new List<string>();
                string name = (string)PokeInfo["name"];
                string pType = (string)PokeInfo["types"][0]["type"]["name"];
                pokemon["Name"] = (name.First().ToString().ToUpper() + name.Substring(1));
                pokemon["Height"] = (string)PokeInfo["height"];
                pokemon["Weight"] = (string)PokeInfo["weight"];
                for(int i=0; i<PokeInfo["types"].Count(); i++){
                    string tWord = (string)PokeInfo["types"][i]["type"]["name"];
                    tempType.Add(tWord.First().ToString().ToUpper() + tWord.Substring(1));
                }
                var spriteURL = new Dictionary<string,object>();
                spriteURL = PokeInfo["sprites"].ToObject<Dictionary<string, object>>();
                foreach(KeyValuePair<string,object> entry in spriteURL)
                {
                    if (entry.Value != null)
                    {
                        tempSprite.Add(entry.Value as string);
                    }
                    else
                    {
                        continue;
                    }
                }
                pokemon["Types"] = tempType;
                pokemon["Sprites"] = tempSprite;
            }
            ).Wait();
            ViewBag.Name = (string)pokemon["Name"];
            ViewBag.Types = (List<string>)pokemon["Types"];
            ViewBag.Height = (string)pokemon["Height"];
            ViewBag.Weight = (string)pokemon["Weight"];
            ViewBag.Sprites = (List<string>)pokemon["Sprites"];
            return View("info");  
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
                var tempType = new List<string>();
                var tempSprite = new List<string>();
                string name = (string)PokeInfo["name"];
                pokemon["name"] = name.First().ToString().ToUpper() + name.Substring(1);
                pokemon["height"] = (string)PokeInfo["height"];
                pokemon["weight"] = (string)PokeInfo["weight"];
                for(int i=0; i<PokeInfo["types"].Count(); i++){
                    string tWord = (string)PokeInfo["types"][i]["type"]["name"];
                    tempType.Add(tWord.First().ToString().ToUpper() + tWord.Substring(1));
                }
                pokemon["types"] = tempType;
                var spriteURL = new Dictionary<string,object>();
                spriteURL = PokeInfo["sprites"].ToObject<Dictionary<string, object>>();
                foreach(KeyValuePair<string,object> entry in spriteURL)
                {
                    if (entry.Value != null)
                    {
                        tempSprite.Add(entry.Value as string);
                    }
                    else
                    {
                        continue;
                    }
                }
                pokemon["sprites"] = tempSprite;
            }
            ).Wait();
            return Json(pokemon);   
        }  
    }
}