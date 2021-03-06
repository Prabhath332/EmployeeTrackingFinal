//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LocationService.svcref {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://webs.mlocatorwebs.mobitel.family/", ConfigurationName="svcref.Provider")]
    public interface Provider {
        
        // CODEGEN: Generating message contract since element name userName from namespace  is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://webs.mlocatorwebs.mobitel.family/Provider/GetLocationByNumberRequest", ReplyAction="http://webs.mlocatorwebs.mobitel.family/Provider/GetLocationByNumberResponse")]
        LocationService.svcref.GetLocationByNumberResponse GetLocationByNumber(LocationService.svcref.GetLocationByNumberRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://webs.mlocatorwebs.mobitel.family/Provider/GetLocationByNumberRequest", ReplyAction="http://webs.mlocatorwebs.mobitel.family/Provider/GetLocationByNumberResponse")]
        System.Threading.Tasks.Task<LocationService.svcref.GetLocationByNumberResponse> GetLocationByNumberAsync(LocationService.svcref.GetLocationByNumberRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetLocationByNumberRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetLocationByNumber", Namespace="http://webs.mlocatorwebs.mobitel.family/", Order=0)]
        public LocationService.svcref.GetLocationByNumberRequestBody Body;
        
        public GetLocationByNumberRequest() {
        }
        
        public GetLocationByNumberRequest(LocationService.svcref.GetLocationByNumberRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class GetLocationByNumberRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string userName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string mobileNumber;
        
        public GetLocationByNumberRequestBody() {
        }
        
        public GetLocationByNumberRequestBody(string userName, string password, string mobileNumber) {
            this.userName = userName;
            this.password = password;
            this.mobileNumber = mobileNumber;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetLocationByNumberResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetLocationByNumberResponse", Namespace="http://webs.mlocatorwebs.mobitel.family/", Order=0)]
        public LocationService.svcref.GetLocationByNumberResponseBody Body;
        
        public GetLocationByNumberResponse() {
        }
        
        public GetLocationByNumberResponse(LocationService.svcref.GetLocationByNumberResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class GetLocationByNumberResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string @return;
        
        public GetLocationByNumberResponseBody() {
        }
        
        public GetLocationByNumberResponseBody(string @return) {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ProviderChannel : LocationService.svcref.Provider, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ProviderClient : System.ServiceModel.ClientBase<LocationService.svcref.Provider>, LocationService.svcref.Provider {
        
        public ProviderClient() {
        }
        
        public ProviderClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ProviderClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProviderClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProviderClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LocationService.svcref.GetLocationByNumberResponse LocationService.svcref.Provider.GetLocationByNumber(LocationService.svcref.GetLocationByNumberRequest request) {
            return base.Channel.GetLocationByNumber(request);
        }
        
        public string GetLocationByNumber(string userName, string password, string mobileNumber) {
            LocationService.svcref.GetLocationByNumberRequest inValue = new LocationService.svcref.GetLocationByNumberRequest();
            inValue.Body = new LocationService.svcref.GetLocationByNumberRequestBody();
            inValue.Body.userName = userName;
            inValue.Body.password = password;
            inValue.Body.mobileNumber = mobileNumber;
            LocationService.svcref.GetLocationByNumberResponse retVal = ((LocationService.svcref.Provider)(this)).GetLocationByNumber(inValue);
            return retVal.Body.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LocationService.svcref.GetLocationByNumberResponse> LocationService.svcref.Provider.GetLocationByNumberAsync(LocationService.svcref.GetLocationByNumberRequest request) {
            return base.Channel.GetLocationByNumberAsync(request);
        }
        
        public System.Threading.Tasks.Task<LocationService.svcref.GetLocationByNumberResponse> GetLocationByNumberAsync(string userName, string password, string mobileNumber) {
            LocationService.svcref.GetLocationByNumberRequest inValue = new LocationService.svcref.GetLocationByNumberRequest();
            inValue.Body = new LocationService.svcref.GetLocationByNumberRequestBody();
            inValue.Body.userName = userName;
            inValue.Body.password = password;
            inValue.Body.mobileNumber = mobileNumber;
            return ((LocationService.svcref.Provider)(this)).GetLocationByNumberAsync(inValue);
        }
    }
}
