using bookstoreagent.Workflow;
using bookstoreagent.Workflow.Agents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bookstoreagent.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class ConversationController : ControllerBase
    //{
    //    private readonly BookRecommendationAgentBuilder _builder;
    //    public ConversationController(BookRecommendationAgentBuilder builder)
    //    {
    //        _builder = builder;
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> CreateSession()
    //    {
    //        var id = Guid.NewGuid().ToString();
    //        return Ok(id);
    //    }


    //    [HttpPost("{sessionId:guid}")]
    //    public async Task<IActionResult> Post([FromRoute] Guid sessionId, string message,CancellationToken ct=default)
    //    {
    //        var agent= _builder.Build();
    //        var session=await _builder.GetSession(agent, sessionId, ct);
    //        var response=await agent.RunAsync(message: message,session, cancellationToken: ct);
    //        return Ok(response.Text);
    //    }
    //}
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly BookOrderWorkflowRunner _runner;
        public ConversationController(BookOrderWorkflowRunner runner)
        {
            _runner = runner;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession()
        {
            var id = Guid.NewGuid().ToString();
            return Ok(id);
        }


        [HttpPost("{conversationId:guid}")]
        public async Task<IActionResult> Post([FromRoute] Guid conversationId, string message, CancellationToken ct = default)
        {
            var response= await _runner.RunAsync(conversationId, message, ct);
            return Ok(response);
        }
    }
}
