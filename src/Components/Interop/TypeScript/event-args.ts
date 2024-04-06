/*
  These classes and their properties must match
  with the classes and properties defined in C# code.
*/

export type UpdateNodeArgs = {
  addNodesData: Array<object>,
  updateNodesData: Array<object>,
  removeNodeId: number | string
}

export type PhotoUploadArgs = {
  fileName: string,
  fileStreamReference: any
}
