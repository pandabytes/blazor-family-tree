
export type CallbackInterop = {
  isAsync: boolean,
  isCallBackInterop: boolean,
  dotNetRef: DotNetObjectReference,
}

export interface DotNetObjectReference {
  invokeMethod(methodName: string, ...args: any[]): any;
  invokeMethodAsync(methodName: string, ...args: any[]): Promise<any>;
}
