﻿/// <reference path="allors.module.ts" />
/// <reference path="../Workspace/Method.ts" />

namespace Allors {
    export class Database {
        constructor(private $http: angular.IHttpService, public $q: angular.IQService, public prefix: string, public postfix: string, public baseUrl: string) {
        }

        authorization: string;

        get headers(): any {
            return this.authorization ? {
                headers: { 'Authorization': this.authorization }
            } : undefined;
        }

        pull(name: string, params?: any): angular.IPromise<Data.PullResponse> {
            return this.$q((resolve, reject) => {

                const serviceName = `${this.baseUrl}/${name}${this.postfix}`;
                this.$http.post(serviceName, params || {}, this.headers)
                    .then((callbackArg: angular.IHttpPromiseCallbackArg<Data.PullResponse>) => {
                        var response = callbackArg.data;
                        response.responseType = Data.ResponseType.Pull;
                        resolve(response);
                    })
                    .catch(e => {
                        reject(e);
                    });

            });
        }

        sync(syncRequest: Data.SyncRequest): angular.IPromise<Data.SyncResponse> {
            return this.$q((resolve, reject) => {

                const serviceName = `${this.baseUrl}${this.prefix}Sync`;
                this.$http.post(serviceName, syncRequest, this.headers)
                    .then((callbackArg: angular.IHttpPromiseCallbackArg<Data.SyncResponse>) => {
                        var response = callbackArg.data;
                        response.responseType = Data.ResponseType.Sync;
                        resolve(response);
                    })
                    .catch(e => {
                        reject(e);
                    });

            });
        }

        push(pushRequest: Data.PushRequest): angular.IPromise<Data.PushResponse> {
            return this.$q((resolve, reject) => {

                const serviceName = `${this.baseUrl}${this.prefix}Push`;
                this.$http.post(serviceName, pushRequest, this.headers)
                    .then((callbackArg: angular.IHttpPromiseCallbackArg<Data.PushResponse>) => {
                        var response = callbackArg.data;
                        response.responseType = Data.ResponseType.Sync;

                        if (response.hasErrors) {
                            reject(response);
                        } else {
                            resolve(response);
                        }
                    })
                    .catch(e => {
                        reject(e);
                    });

            });
        }

        invoke(method: Method): angular.IPromise<Data.InvokeResponse>;
        invoke(methods: Method[], options: Data.InvokeOptions): angular.IPromise<Data.InvokeResponse>;
        invoke(service: string, args?: any): angular.IPromise<Data.InvokeResponse>;
        invoke(methodOrService: Method | Method[] | string, args?: any): angular.IPromise<Data.InvokeResponse> {
            if (methodOrService instanceof Method) {
                return this.invokeMethods([methodOrService]);
            } else if (methodOrService instanceof Array) {
                return this.invokeMethods(methodOrService, args);
            } else {
                return this.invokeService(methodOrService, args);
            }
        }

        private invokeMethods(methods: Method[], options?: Data.InvokeOptions): angular.IPromise<Data.InvokeResponse> {

            return this.$q((resolve, reject) => {

                const invokeRequest: Data.InvokeRequest = {
                    i: methods.map(v => {
                        return {
                            i: v.object.id,
                            m: v.name,
                            v: v.object.version,
                        };
                    }),
                    o: options
                };

                const serviceName = `${this.baseUrl}${this.prefix}Invoke`;
                this.$http.post(serviceName, invokeRequest, this.headers)
                    .then((callbackArg: angular.IHttpPromiseCallbackArg<Data.InvokeResponse>) => {
                        var response = callbackArg.data;
                        response.responseType = Data.ResponseType.Invoke;

                        if (response.hasErrors) {
                            reject(response);
                        } else {
                            resolve(response);
                        }
                    })
                    .catch(e => {
                        reject(e);
                    });

            });

        }

        private invokeService(methodOrService: string, args?: any): angular.IPromise<Data.InvokeResponse> {
            return this.$q((resolve, reject) => {

                const serviceName = this.baseUrl + methodOrService + this.postfix;
                this.$http.post(serviceName, args, this.headers)
                    .then((callbackArg: angular.IHttpPromiseCallbackArg<Data.InvokeResponse>) => {
                        var response = callbackArg.data;
                        response.responseType = Data.ResponseType.Invoke;

                        if (response.hasErrors) {
                            reject(response);
                        } else {
                            resolve(response);
                        }
                    })
                    .catch(e => {
                        reject(e);
                    });

            });
        }
    }
}