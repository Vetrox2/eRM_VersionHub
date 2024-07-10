import { Module } from './module.model';

export interface Version {
  ID: string;
  Modules: Module[];
  Tag: 'preview' | 'scoped' | 'none';
}
