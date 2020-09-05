using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using System.Web.Http;

namespace AuthorizeNetAPI.Controllers
{
    [RoutePrefix("api/AuthorizeNet")]
    public class HomeController : ApiController
    {

        [Route("GetFormToken")]
        [HttpGet]
        public string GetFormToken(decimal amount)
        {

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = "35WkY7AsU",
                ItemElementName = ItemChoiceType.transactionKey,
                Item = "85T8Hsv4JMu76P5b"
            };

            var customerProfile = new customerProfilePaymentType();
            customerProfile.customerProfileId = "1927580170";
            customerProfile.createProfile = true;

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // authorize capture only
                amount = amount,
                profile = customerProfile
                ,
                billTo = new customerAddressType
                {
                    firstName = "firstname",
                    lastName = "lastname",
                    address = "address",
                    city = "city",
                    state = "state",
                    zip = "zip"

                }
                //order = new orderType
                //{
                //    invoiceNumber = "INV-123456",
                //    description = "TEST INVOICE",
                //    purchaseOrderDateUTC= DateTime.Now.ToUniversalTime()
                //}
            };

            var settings = new List<settingType>();
            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedPaymentButtonOptions.ToString(),
                settingValue = @"{""text"": ""Pay""}"
            });

            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedPaymentPaymentOptions.ToString(),
                settingValue = @"{""cardCodeRequired"": true, ""showCreditCard"": true, ""showBankAccount"": true}"


            });

            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedPaymentReturnOptions.ToString(),
                settingValue = @"{""showReceipt"": false }"
            });

            //settings.Add(new settingType()
            //{
            //    settingName = settingNameEnum.hostedPaymentCustomerOptions.ToString(),
            //    settingValue = @"{""addPaymentProfile"": true }"
            //});

            //settings.Add(new settingType()
            //{
            //    settingName = settingNameEnum.hostedPaymentReturnOptions.ToString(),
            //    settingValue = @"{""showReceipt"": true, ""url"": ""http://localhost:4201/summary"", 
            //                     ""urlText"": ""Order Summary"", ""cancelUrl"": ""http://localhost:4201/checkout"",
            //                     ""cancelUrlText"": ""Cancel"" }"
            //});

            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedPaymentOrderOptions.ToString(),
                settingValue = @"{""show"": true}"
            });
            //settings.Add(new settingType()
            //{
            //    settingName = settingNameEnum.hostedPaymentShippingAddressOptions.ToString(),
            //    settingValue = @"{""show"" : false }"
            //});
            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedPaymentBillingAddressOptions.ToString(),
                settingValue = @"{""show"" : false, ""required"" : false }"
            });

            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedPaymentStyleOptions.ToString(),
                settingValue = @"{""bgColor"" : ""green""}"
            });

            //settings.Add(new settingType()
            //{
            //    settingName = settingNameEnum.hostedProfileHeadingBgColor.ToString(),
            //    settingValue =  @"{""green"" }"
            //});

            //settings.Add(new settingType()
            //{
            //    settingName = settingNameEnum.hostedProfilePageBorderVisible.ToString(),
            //    settingValue = @"{false}"
            //});
            //string Url= ""http://localhost:4201/IFrameCommunicator.html"";

            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedPaymentIFrameCommunicatorUrl.ToString(),
                settingValue = @"{ ""url"" : ""http://localhost:4201/IFrameCommunicator.html"" }"
            });


            var request = new getHostedPaymentPageRequest();
            request.transactionRequest = transactionRequest;
            request.hostedPaymentSettings = settings.ToArray();

            // instantiate the controller that will call the service
            var controller = new getHostedPaymentPageController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();


            return response.token;
        }

        [Route("GetCustomerID")]
        [HttpGet]
        public string GetCustomerProfile()
        {


            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = "35WkY7AsU",
                ItemElementName = ItemChoiceType.transactionKey,
                Item = "85T8Hsv4JMu76P5b"
            };

            customerProfileType customerProfile = new customerProfileType();
            //customerProfile.merchantCustomerId = "Test CustomerID";
            customerProfile.email = "abc@gmail.com";
            customerProfile.description = "abc";


            var request = new createCustomerProfileRequest { profile = customerProfile, validationMode = validationModeEnum.none };

            // instantiate the controller that will call the service
            var controller = new createCustomerProfileController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            createCustomerProfileResponse response = controller.GetApiResponse();

            // validate response 
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    return response.customerProfileId;
                }
                else
                {
                    return $"{response.messages.message[0].code}+ and {response.messages.message[0].text}";

                }
            }
            else
            {
                if (controller.GetErrorResponse().messages.message.Length > 0)
                {
                    return $"{response.messages.message[0].code}+ and {response.messages.message[0].text}";
                }
                else
                {
                    return $"Profile created failed";
                }
            }



        }

        [Route("CreateProfile")]
        [HttpGet]
        public ANetApiResponse CreateProfile(string transactionId)
        {
            Console.WriteLine("CreateCustomerProfileFromTransaction Sample");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = "35WkY7AsU",
                ItemElementName = ItemChoiceType.transactionKey,
                Item = "85T8Hsv4JMu76P5b"
            };

            var customerProfile = new customerProfileBaseType
            {
                //merchantCustomerId = "123212",
                email = "hello@castleblack.com",
                description = "This is a sample customer profile"
            };

            var request = new createCustomerProfileFromTransactionRequest
            {
                transId = transactionId,
                // You can either specify the customer information in form of customerProfileBaseType object
                customer = customerProfile
                //  OR   
                // You can just provide the customer Profile ID
                // customerProfileId = "123343"                
            };

            var controller = new createCustomerProfileFromTransactionController(request);
            controller.Execute();

            createCustomerProfileResponse response = controller.GetApiResponse();

            //// validate response
            //if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            //{
            //    if (response != null && response.messages.message != null)
            //    {
            //        Console.WriteLine("Success, CustomerProfileID : " + response.customerProfileId);
            //        Console.WriteLine("Success, CustomerPaymentProfileID : " + response.customerPaymentProfileIdList[0]);
            //        Console.WriteLine("Success, CustomerShippingProfileID : " + response.customerShippingAddressIdList[0]);
            //    }
            //}
            //else if (response != null)
            //{
            //    Console.WriteLine("Error: " + response.messages.message[0].code + "  " + response.messages.message[0].text);
            //}

            return response;
        }

        [Route("GetTransactionDetails")]
        [HttpGet]
        public ANetApiResponse GetTransactionDetails(string transactionId)
        {


            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = "35WkY7AsU",
                ItemElementName = ItemChoiceType.transactionKey,
                Item = "85T8Hsv4JMu76P5b"
            };

            var request = new getTransactionDetailsRequest();
            request.transId = transactionId;

            // instantiate the controller that will call the service
            var controller = new getTransactionDetailsController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();
            return response;
        }

        [Route("RefundTransaction")]
        [HttpGet]
        public ANetApiResponse RefundTransaction(decimal TransactionAmount, string TransactionID, string cardNumber)
        {
            Console.WriteLine("Refund Transaction");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = "35WkY7AsU",
                ItemElementName = ItemChoiceType.transactionKey,
                Item = "85T8Hsv4JMu76P5b"
            };

            var creditCard = new creditCardType
            {
                cardNumber = cardNumber,
                expirationDate = "XXXX"
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.refundTransaction.ToString(),    // refund type
                payment = paymentType,
                amount = TransactionAmount,
                refTransId = TransactionID
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            // validate response


            return response;
        }

        [Route("GetEditPaymentToken")]
        [HttpGet]
        public string GetEditPaymentToken()
        {

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = "35WkY7AsU",
                ItemElementName = ItemChoiceType.transactionKey,
                Item = "85T8Hsv4JMu76P5b"
            };

            var customerProfile = new customerProfilePaymentType();
            customerProfile.customerProfileId = "1927580170";
            customerProfile.createProfile = true;

            var settings = new List<settingType>();
            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedProfileSaveButtonText.ToString(),
                settingValue = "Save"
            });

            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedProfileIFrameCommunicatorUrl.ToString(),
                settingValue = "http://localhost:4201/IFrameCommunicator.html"
            });

            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedProfileManageOptions.ToString(),
                settingValue = "showPayment"
            });

            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedProfilePaymentOptions.ToString(),
                settingValue = "showCreditCard"
            });

            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedProfileCardCodeRequired.ToString(),
                settingValue = "true"
            });

            settings.Add(new settingType()
            {
                settingName = settingNameEnum.hostedProfileBillingAddressOptions.ToString(),
                settingValue = "showNone"
            });



            var request = new getHostedProfilePageRequest();
            request.customerProfileId = "1927580170";
            request.hostedProfileSettings = settings.ToArray();

            // instantiate the controller that will call the service
            var controller = new getHostedProfilePageController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();


            return response.token;
        }

    }
}
