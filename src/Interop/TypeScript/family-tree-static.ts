/// <reference path="types/FamilyTree.d.ts" />

export class FamilyTreeStaticInterop {
  public getBuiltInIcon(iconType: string, width: number, height: number, color: string) : string {
    const validIconTypes = new Set<string>(Object.keys(FamilyTree.icon));
    if (!validIconTypes.has(iconType))
    {
      return '';
    }

    return FamilyTree.icon[iconType](width, height, color);
  }
}
