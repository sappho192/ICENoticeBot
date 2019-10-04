using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

/*
 * Get updates: https://api.telegram.org/botXXXXXXXXX:YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY/getUpdates
 * Update example: {"update_id":12345678,"message":{"message_id":2,"from":{"id":123456789,"is_bot":false,"first_name":"\ud0dc\uc778","last_name":"\uae40","username":"abcdefgh","language_code":"ko"},"chat":{"id":123456789,"first_name":"\ud0dc\uc778","last_name":"\uae40","username":"abcdefgh","type":"private"},"date":1234567890,"text":"test"}}
 *   chat_id is "id" right after "from"
 * Send reply: https://api.telegram.org/botXXXXXXXXX:YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY/sendMessage?chat_id=123456789&reply_to_message_id=2&text=Hello%20World
 * Send message: https://api.telegram.org/botXXXXXXXXX:YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY/sendMessage?chat_id=123456789&text=Hello%20World2
 */

namespace ICENoticeBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
