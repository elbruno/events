using System;
using System.Globalization;
using System.Threading.Tasks;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;

namespace AlexaOltivaTest01.Alexa
{
    public class AlexaResponseAsync : SpeechletAsync
    {
        public override Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session)
        {
            var onLaunch = "You just launch this amazing skill from McMaster University. Thanks Anand.";
            return Task.FromResult(CompileResponse(onLaunch));
        }

        public async override Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session)
        {
            var intentName = intentRequest.Intent.Name;
            var description = $"On Intent {intentName}";

            var closingDate = DateTime.Now.AddDays(1).ToShortDateString(); ;
            switch (intentName)
            {
                case "AMAZON.HelpIntent":
                    description = GetHelpText();
                    break;
                case "GetNewTipIntent":
                    description = $@"The right way to use toilet paper is Bruno's way!";
                    break;
                case "DebugIntentHistoryIntent":
                    description = "DEBUG MODE. The latests requested Intents are ";
                    foreach (var item in session.IntentSequence)
                    {
                        description += item + ", ";
                    }
                    break;
            }
            var response = CompileResponse(description);
            return await Task.FromResult(response);
        }

        public string GetHelpText() {
            return $@"You can ask several questions to oltiva Test. In example:
Tell me a new tip.
Tell me a cool suggestion for my home.";
        }
        public override Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session)
        {
            return Task.FromResult(0);
        }

        public override Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session)
        {
            return Task.FromResult(0);
        }

        public static SpeechletResponse CompileResponse(string output)
        {
            //var response = new SpeechletResponse
            //{
            //    OutputSpeech = new PlainTextOutputSpeech { Text = output },
            //    ShouldEndSession = true
            //};
            //return response;

            var card = new SimpleCard {Title = "Home Tips"};

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            card.Content = textInfo.ToTitleCase(output);

            // Create the plain text output.
            var speech = new PlainTextOutputSpeech {Text = output };

            // Create the speechlet response.
            var response = new SpeechletResponse
            {
                ShouldEndSession = true,
                OutputSpeech = speech,
                Card = card
            };
            return response;
        }
    }
}