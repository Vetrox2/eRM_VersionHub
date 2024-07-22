import { Module } from './module.model';

export interface Version {
  ID: string;
  Number:string;
  Modules: Module[];
  PublishedTag: Tag;
  Name: string;
}
export type Tag =  "" | "none" | "preview" | "scoped" ;
