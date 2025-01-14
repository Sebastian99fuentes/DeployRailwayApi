using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDeployReservas.Controllers.Dtos.Implemento;
using ApiDeployReservas.Controllers.Helpers;
using ApiDeployReservas.Data;
using ApiDeployReservas.Repository.Interfaces;
using ApiDeployReservas.Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ApiDeployReservas.Repository
{
    public class ImplementoRepository : IImplementosRepository
    {
        private readonly  ApplicationDBContext _context;

        public ImplementoRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<List<Implemento>> GetallAsync()
        {
             var implementos = _context.Implemento.AsQueryable();

          

             return  await  implementos.ToListAsync();

        } 

        public async Task<Implemento?> CreateAsync(Implemento implemento)
        {
             await _context.Implemento.AddAsync(implemento);
             await _context.SaveChangesAsync();

             return implemento;
        }

        public  async Task<Implemento?> UpdateAsync(Guid id, CreateImplementoRequestDto implementoDto)
        {
           var existingImplemento = await _context.Implemento.FirstOrDefaultAsync(i => i.Id == id);
               if(existingImplemento == null)
                {
                    return null;
                }
               
               existingImplemento.NombreImple = implementoDto.NombreImple;
               existingImplemento.Cantidad = implementoDto.Cantidad;

            
                await _context.SaveChangesAsync();

               return existingImplemento; 

        }

         public async Task<Implemento?> DeleteAsync(Guid id)
        {
           var implemento = _context.Implemento.FirstOrDefault(i => i.Id == id);
           if(implemento == null)
           {
            return null;
           }

           _context.Remove(implemento);
           await _context.SaveChangesAsync();

           return implemento;
        }

        public async Task<Implemento?> GetByIdAsync(Guid id)
        {
           return await _context.Implemento.AsQueryable().FirstOrDefaultAsync(x => x.Id== id);
        }

        public Task<bool> Exist(Guid id)
        {
             return _context.Implemento.AnyAsync(a => a.Id ==id);
        }

        public async Task<Implemento?> UpdateImpleAsync(Guid? id, bool upDown)
        {
            var existingImplemento = await _context.Implemento.FirstOrDefaultAsync(i => i.Id == id);
               if(existingImplemento == null)
                {
                    return null;
                }
                if(upDown)
                {       
                    existingImplemento.Cantidad++;
                } 
                else
                {
                    existingImplemento.Cantidad--;
                }
            

                await _context.SaveChangesAsync();

               return existingImplemento; 
        }
    }
}